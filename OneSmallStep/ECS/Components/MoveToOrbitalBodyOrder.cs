using System;
using System.Windows;
using GoldenAnvil.Utility.Logging;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.Utility;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS.Components
{
	public sealed class MoveToOrbitalBodyOrder : MovementOrderBase
	{
		public MoveToOrbitalBodyOrder(EntityId targetEntityId, double speedPerTick)
		{
			TargetEntityId = targetEntityId;
			SpeedPerTick = speedPerTick;
		}

		public EntityId TargetEntityId { get; }

		public double SpeedPerTick { get; }

		public Point? InterceptPoint { get; private set; }

		public override bool PrepareIntercept(IEntityLookup entityLookup, Point currentAbsolutePosition, double maxSpeedPerTick, TimePoint currentTime)
		{
			if (InterceptPoint.HasValue)
				return false;

			var targetEntity = entityLookup.GetEntity(TargetEntityId);
			var targetOrders = targetEntity.GetOptionalComponent<MovementOrdersComponent>();
			var targetUnitDesign = targetEntity.GetOptionalComponent<OrbitalUnitDesignComponent>();
			if (MovementOrderUtility.CanExecuteOrders(targetOrders, targetUnitDesign))
			{
				throw new NotImplementedException();
			}
			else
			{
				var speedPerTick = Math.Min(SpeedPerTick, maxSpeedPerTick);
				InterceptPoint = MovementOrderUtility.GetInterceptPoint(entityLookup, currentAbsolutePosition, speedPerTick, targetEntity, currentTime);
			}

			return true;
		}

		public override bool TryMarkAsResolved(IEntityLookup entityLookup, Point currentAbsolutePosition)
		{
			if (currentAbsolutePosition != InterceptPoint)
				return false;

			var targetEntity = entityLookup.GetEntity(TargetEntityId);
			var targetPosition = targetEntity.GetRequiredComponent<EllipticalOrbitalPositionComponent>();
			if (currentAbsolutePosition.IsWithinOneMeter(targetPosition.GetCurrentAbsolutePosition(entityLookup)))
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

		public override MovementOrderBase Clone()
		{
			return new MoveToOrbitalBodyOrder(TargetEntityId, SpeedPerTick) { InterceptPoint = InterceptPoint };
		}

		static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(MoveToOrbitalBodyOrder));
	}
}
