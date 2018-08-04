using System;
using System.Windows;
using GoldenAnvil.Utility.Logging;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.Utility;

namespace OneSmallStep.ECS.Components
{
	public sealed class MoveToOrbitalBodyOrder : MovementOrderBase
	{
		public MoveToOrbitalBodyOrder(Entity targetEntity, double speedPerTick)
		{
			TargetEntity = targetEntity;
			SpeedPerTick = speedPerTick;
		}

		public Entity TargetEntity { get; }

		public double SpeedPerTick { get; }

		public Point? InterceptPoint { get; private set; }

		public override void PrepareIntercept(Point currentAbsolutePosition, double maxSpeedPerTick)
		{
			if (InterceptPoint.HasValue)
				return;

			var targetOrders = TargetEntity.GetOptionalComponent<MovementOrdersComponent>();
			var targetUnitDesign = TargetEntity.GetOptionalComponent<OrbitalUnitDesignComponent>();
			if (MovementOrderUtility.CanExecuteOrders(targetOrders, targetUnitDesign))
			{
				throw new NotImplementedException();
			}
			else
			{
				var speedPerTick = Math.Min(SpeedPerTick, maxSpeedPerTick);
				InterceptPoint = MovementOrderUtility.GetInterceptPoint(currentAbsolutePosition, speedPerTick, TargetEntity);
			}
		}

		public override bool TryMarkAsResolved(Point currentAbsolutePosition)
		{
			if (currentAbsolutePosition != InterceptPoint)
				return false;

			var targetPosition = TargetEntity.GetRequiredComponent<OrbitalPositionComponent>();
			if (currentAbsolutePosition.IsWithinOneMeter(targetPosition.GetCurrentAbsolutePosition()))
				return true;

			InterceptPoint = null;
			return false;
		}

		public override Point MoveOneTick(Point currentAbsolutePosition)
		{
			if (InterceptPoint == null)
				return currentAbsolutePosition;

			var vector = currentAbsolutePosition.VectorTo(InterceptPoint.Value);
			if (vector.LengthSquared < SpeedPerTick * SpeedPerTick)
			{
				Log.Info($"Reached target point ({InterceptPoint.Value.X}, {InterceptPoint.Value.Y})");
				return InterceptPoint.Value;
			}

			vector.Normalize();
			return currentAbsolutePosition + (vector * SpeedPerTick);
		}

		private static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(MoveToOrbitalBodyOrder));
	}
}
