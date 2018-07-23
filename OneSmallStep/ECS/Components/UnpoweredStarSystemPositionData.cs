using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Logging;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.Utility;

namespace OneSmallStep.ECS.Components
{
	public sealed class UnpoweredStarSystemPositionData : StarSystemPositionDataBase
	{
		public double? Mu { get; set; }
		public double? OrbitalRadius { get; set; }
		public double? AngularVelocity { get; set; }

		public override Point GetAbsolutePosition(OrbitalPositionComponent body)
		{
			var position = body.RelativePosition;
			var parentPosition = body.Parent?.GetComponent<OrbitalPositionComponent>()?.GetAbsolutePosition();
			if (parentPosition != null)
				position.Offset(parentPosition.Value.X, parentPosition.Value.Y);
			return position;
		}

		public override Point GetAbsolutePositionAtTime(OrbitalPositionComponent body, double ticks)
		{
			var parentPosition = body.Parent?.GetComponent<OrbitalPositionComponent>()?.GetAbsolutePositionAtTime(ticks);
			if (parentPosition != null)
			{
				var currentAngle = Math.Atan2(body.RelativePosition.Y, body.RelativePosition.X);
				var angle = currentAngle + (AngularVelocity.Value * ticks * Constants.SecondsPerTick);
				return new Point(Math.Cos(angle) * OrbitalRadius.Value + parentPosition.Value.X, Math.Sin(angle) * OrbitalRadius.Value + parentPosition.Value.Y);
			}

			return body.RelativePosition;
		}

		public override void EnsureStartValidity(OrbitalPositionComponent body)
		{
			var parentBody = body.Parent?.GetComponent<OrbitalPositionComponent>();

			if (parentBody != null)
			{
				if (!Mu.HasValue)
					Mu = Constants.GravitationalConstant * (body.Mass + parentBody.Mass);
				if (!OrbitalRadius.HasValue)
					OrbitalRadius = new Vector(body.RelativePosition.X, body.RelativePosition.Y).Length;
				if (!AngularVelocity.HasValue)
					AngularVelocity = Math.Sqrt(Math.Pow(OrbitalRadius.Value, 3.0) / Mu.Value);
			}
		}

		public override void EnsureEndValidity(OrbitalPositionComponent body)
		{
		}

		public override void MoveOneTick(OrbitalPositionComponent body)
		{
			var parentBody = body.Parent?.GetComponent<OrbitalPositionComponent>();

			if (parentBody != null)
			{
				var currentAngle = Math.Atan2(body.RelativePosition.Y, body.RelativePosition.X);
				var newAngle = currentAngle + AngularVelocity.Value * Constants.SecondsPerTick;
				body.RelativePosition = new Point(OrbitalRadius.Value * Math.Cos(newAngle), OrbitalRadius.Value * Math.Sin(newAngle));
			}
		}

		public override Point? GetInterceptPoint(OrbitalPositionComponent body, Point interceptorPosition, double interceptorMaxSpeed)
		{
			if (body.Parent == null)
				return GetAbsolutePosition(body);

			double? interceptorDistanceToCenter = null;
			double longestDistance = 0.0;
			double? minDistanceToCenter = null;
			double? maxDistanceToCenter = null;
			foreach (var parentBody in body.EnumerateThisAndParents().Reverse())
			{
				if (interceptorDistanceToCenter == null)
				{
					interceptorDistanceToCenter = interceptorPosition.DistanceTo(parentBody.RelativePosition);
					longestDistance = interceptorDistanceToCenter.Value;
				}
				else
				{
					var orbitaRadius = parentBody.RelativePosition.DistanceTo(new Point());
					longestDistance += orbitaRadius;
					minDistanceToCenter = minDistanceToCenter.HasValue ? minDistanceToCenter - orbitaRadius : orbitaRadius;
					maxDistanceToCenter = maxDistanceToCenter.HasValue ? maxDistanceToCenter + orbitaRadius : orbitaRadius;
				}
			}
			double shortestDistance;
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

			int minTimeInTicks = (int) Math.Floor(shortestDistance / interceptorMaxSpeed);
			int maxTimeInTicks = (int) Math.Ceiling(longestDistance / interceptorMaxSpeed);

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
				var positionDifference = GetPositionDifference(body, interceptorPosition, interceptorMaxSpeed, timeInTicks);
				if (positionDifference >= 0.0 && positionDifference < interceptorMaxSpeed)
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

			var targetPoint = GetAbsolutePositionAtTime(body, timeInTicks);
			if (failedIntercept)
			{
				var vector = interceptorPosition.VectorTo(targetPoint);
				targetPoint = interceptorPosition + (vector * 0.5);
				Log.Info($"Failed intercept.");
			}

			Log.Info($"Targeting point ({targetPoint.X}, {targetPoint.Y}), will reach in {timeInTicks} ticks from ({interceptorPosition.X}, {interceptorPosition.Y}).");

			return targetPoint;
		}

		private double GetPositionDifference(OrbitalPositionComponent body, Point interceptorPosition, double interceptorSpeed, int tick)
		{
			var targetPosition = GetAbsolutePositionAtTime(body, tick);
			var v1 = new Vector(interceptorPosition.X, interceptorPosition.Y);
			var v2 = new Vector(targetPosition.X, targetPosition.Y);
			var v = v2 - v1;
			var distanceToTarget = v.Length;
			v.Normalize();
			v = v * interceptorSpeed * tick;
			return v.Length - distanceToTarget;
		}

		private double GetError(OrbitalPositionComponent body, Point interceptorPosition, double interceptorSpeed, double tick)
		{
			var targetPosition = GetAbsolutePositionAtTime(body, tick);
			var dx = targetPosition.X - interceptorPosition.X;
			var dy = targetPosition.Y - interceptorPosition.Y;

			return (((dx * dx) + (dy * dy)) / (interceptorSpeed * interceptorSpeed)) - (tick * tick);
		}

		static readonly ILogSource Log = LogManager.CreateLogSource(nameof(UnpoweredStarSystemPositionData));
	}
}
