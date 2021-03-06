using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GoldenAnvil.Utility.Logging;
using GoldenAnvil.Utility.Windows;
using JetBrains.Annotations;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS.Components
{
	public static class MovementOrderUtility
	{
		public static bool CanExecuteOrders([CanBeNull] OrdersComponent orders, [CanBeNull] OrbitalUnitDesignComponent unitDesign)
		{
			return (orders?.HasActiveOrder<MovementOrderBase>() ?? false) &&
				((unitDesign?.MaxSpeedPerTick ?? 0) > 0);
		}

		public static Point GetInterceptPoint(IEntityLookup entityLookup, Point interceptorPosition, double interceptorMaxSpeedPerTick, [NotNull] Entity targetEntity, TimePoint currentTime)
		{
			var targetPosition = targetEntity.GetRequiredComponent<EllipticalOrbitalPositionComponent>();
			if (!targetPosition.ParentId.HasValue)
				return targetPosition.GetCurrentAbsolutePosition(entityLookup);

			double longestDistance = 0.0;
			double shortestDistance = 0.0;
			double? interceptorDistanceToCenter = null;
			//double? minDistanceToCenter = null;
			double? maxDistanceToCenter = null;
			foreach (var parentBody in targetPosition.EnumerateThisAndParents(entityLookup).Reverse())
			{
				if (interceptorDistanceToCenter == null)
				{
					interceptorDistanceToCenter = interceptorPosition.DistanceTo(parentBody.RelativePosition);
					longestDistance = interceptorDistanceToCenter.Value;
				}
				else
				{
					var maxDistance = parentBody.Focus.Value + parentBody.SemiMajorAxis.Value;
					longestDistance += maxDistance;
					//minDistanceToCenter = minDistanceToCenter.HasValue ? minDistanceToCenter - maxDistance : maxDistance;
					maxDistanceToCenter = maxDistanceToCenter.HasValue ? maxDistanceToCenter + maxDistance : maxDistance;
				}
			}
			// TODO: Use minimum distance to narrow down results more quickly
			/*
			if (maxDistanceToCenter.HasValue)
			{
				if (interceptorDistanceToCenter.Value > maxDistanceToCenter.Value)
					shortestDistance = interceptorDistanceToCenter.Value - maxDistanceToCenter.Value;
				else if (interceptorDistanceToCenter.Value < minDistanceToCenter.Value)
					shortestDistance = minDistanceToCenter.Value - interceptorDistanceToCenter.Value;
				else
					shortestDistance = 0.0;
			}
			else
			{
				shortestDistance = interceptorDistanceToCenter.Value;
			}
			*/
			int minTimeInTicks = (int) Math.Floor(shortestDistance / interceptorMaxSpeedPerTick);
			int maxTimeInTicks = (int) Math.Ceiling(longestDistance / interceptorMaxSpeedPerTick);

			bool failedIntercept = false;
			var timeInTicks = minTimeInTicks;
			Dictionary<int, double> tickToPosition = new Dictionary<int, double>();
			while (minTimeInTicks != maxTimeInTicks)
			{
				var newTimeInTicks = (minTimeInTicks + maxTimeInTicks) / 2;
				if (tickToPosition.ContainsKey(newTimeInTicks))
				{
					timeInTicks = Math.Max(timeInTicks, newTimeInTicks);
					failedIntercept = true;
					break;
				}

				timeInTicks = newTimeInTicks;
				var positionDifference = GetPositionDifference(entityLookup, targetPosition, interceptorPosition, interceptorMaxSpeedPerTick, timeInTicks, currentTime);
				if (positionDifference >= 0.0 && positionDifference < interceptorMaxSpeedPerTick)
					break;

				tickToPosition[timeInTicks] = positionDifference;

				if (positionDifference > 0)
					maxTimeInTicks = timeInTicks;
				else if (minTimeInTicks == timeInTicks)
					minTimeInTicks++;
				else
					minTimeInTicks = timeInTicks;
			}

			if (!failedIntercept)
			{
				if (minTimeInTicks == maxTimeInTicks)
					timeInTicks = minTimeInTicks;
			}

			var targetPoint = targetPosition.GetAbsoluteOrbitalPositionAtTime(entityLookup, currentTime + new TimeOffset(timeInTicks));
			if (failedIntercept)
			{
				var vector = interceptorPosition.VectorTo(targetPoint);
				targetPoint = interceptorPosition + (vector * 0.5);
				Log.Info("Failed intercept.");
			}

			Log.Info($"Targeting point ({targetPoint.X}, {targetPoint.Y}), will reach in {timeInTicks} ticks from ({interceptorPosition.X}, {interceptorPosition.Y}).");

			return targetPoint;
		}

		private static double GetPositionDifference(IEntityLookup entityLookup, EllipticalOrbitalPositionComponent targetPosition, Point interceptorPosition, double interceptorSpeedPerTick, int tick, TimePoint currentTime)
		{
			var targetAbsolutePosition = targetPosition.GetAbsoluteOrbitalPositionAtTime(entityLookup, currentTime + new TimeOffset(tick));
			var v1 = new Vector(interceptorPosition.X, interceptorPosition.Y);
			var v2 = new Vector(targetAbsolutePosition.X, targetAbsolutePosition.Y);
			var v = v2 - v1;
			var distanceToTarget = v.Length;
			v.Normalize();
			v = v * interceptorSpeedPerTick * tick;
			return v.Length - distanceToTarget;
		}

		private static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(MovementOrderUtility));
	}
}
