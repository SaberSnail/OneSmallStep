using System;
using System.Windows;
using GoldenAnvil.Utility.Logging;
using GoldenAnvil.Utility.Windows;
using JetBrains.Annotations;
using OneSmallStep.Utility;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS.Components
{
	public sealed class MoveToOrbitalBodyOrder : MovementOrderBase
	{
		public MoveToOrbitalBodyOrder(OrderId orderId, EntityId targetEntityId, double speedPerTick)
			: this(orderId, targetEntityId, speedPerTick, null)
		{
		}

		public EntityId TargetEntityId { get; }

		public double SpeedPerTick { get; }

		public Point? InterceptPoint { get; }

		public override MovementOrderBase PrepareIntercept(IEntityLookup entityLookup, Point currentAbsolutePosition, double maxSpeedPerTick, TimePoint currentTime)
		{
			if (InterceptPoint.HasValue)
				return null;

			MoveToOrbitalBodyOrder newOrder = null;
			var targetEntity = entityLookup.GetEntity(TargetEntityId);
			var targetOrders = targetEntity.GetOptionalComponent<OrdersComponent>();
			var targetUnitDesign = targetEntity.GetOptionalComponent<OrbitalUnitDesignComponent>();
			if (MovementOrderUtility.CanExecuteOrders(targetOrders, targetUnitDesign))
			{
				throw new NotImplementedException();
			}
			else
			{
				var speedPerTick = Math.Min(SpeedPerTick, maxSpeedPerTick);
				var interceptPoint = MovementOrderUtility.GetInterceptPoint(entityLookup, currentAbsolutePosition, speedPerTick, targetEntity, currentTime);
				newOrder = CloneWithIntercept(interceptPoint);
			}

			return newOrder;
		}

		public override bool TryMarkAsResolved(IEntityLookup entityLookup, Point currentAbsolutePosition, out MovementOrderBase newOrder)
		{
			newOrder = null;

			if (currentAbsolutePosition != InterceptPoint)
				return false;

			var targetEntity = entityLookup.GetEntity(TargetEntityId);
			var targetPosition = targetEntity.GetRequiredComponent<EllipticalOrbitalPositionComponent>();
			var targetAbsolutePosition = targetPosition.GetCurrentAbsolutePosition(entityLookup);
			if (currentAbsolutePosition.IsWithinOneMeter(targetAbsolutePosition))
				return true;

			Log.Info($"Reached InterceptPoint ({InterceptPoint.Value.X}, {InterceptPoint.Value.Y}), but target is not nearby ({targetAbsolutePosition.X}, {targetAbsolutePosition.Y})");
			newOrder = CloneWithIntercept(null);
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

		public override OrderBase Clone() => new MoveToOrbitalBodyOrder(this, this.InterceptPoint);

		private MoveToOrbitalBodyOrder CloneWithIntercept(Point? interceptPoint) => new MoveToOrbitalBodyOrder(this, interceptPoint);

		private MoveToOrbitalBodyOrder([NotNull] MoveToOrbitalBodyOrder that, Point? interceptPoint)
			: base(that)
		{
			TargetEntityId = that.TargetEntityId;
			SpeedPerTick = that.SpeedPerTick;
			InterceptPoint = interceptPoint;
		}

		private MoveToOrbitalBodyOrder(OrderId orderId, EntityId targetEntityId, double speedPerTick, Point? interceptPoint)
			: base(orderId)
		{
			TargetEntityId = targetEntityId;
			SpeedPerTick = speedPerTick;
			InterceptPoint = interceptPoint;
		}

		static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(MoveToOrbitalBodyOrder));
	}
}
