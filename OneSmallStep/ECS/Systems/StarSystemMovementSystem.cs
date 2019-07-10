using System.Collections.Generic;
using System.Globalization;
using GoldenAnvil.Utility.Logging;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.ECS.Components;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS.Systems
{
	public sealed class StarSystemMovementSystem : SystemBase
	{
		public override void ProcessTick(IEntityLookup entityLookup, NotificationLog notificationLog, TimePoint newTime)
		{
			var entities = entityLookup.GetEntitiesMatchingKey(entityLookup.CreateComponentKey<EllipticalOrbitalPositionComponent>());
			var entitiesWithMovementOrders = new Dictionary<EntityId, Entity>();

			foreach (var entity in entities)
			{
				var unitDesign = entity.GetOptionalComponent<OrbitalUnitDesignComponent>();
				var orders = entity.GetOptionalComponent<OrdersComponent>();
				if (MovementOrderUtility.CanExecuteOrders(orders, unitDesign))
				{
					entitiesWithMovementOrders.Add(entity.Id, entity);
				}
				else
				{
					var position = entity.GetRequiredComponent<EllipticalOrbitalPositionComponent>();
					var body = entity.GetRequiredComponent<OrbitalBodyCharacteristicsComponent>();
					position.InitializeOrbitalValuesIfNeeded(entityLookup, body.Mass);
				}
			}

			var currentTime = newTime - new TimeOffset(1);
			foreach (var entity in entitiesWithMovementOrders.Values)
			{
				var position = entity.GetRequiredComponent<EllipticalOrbitalPositionComponent>();
				var orders = entity.GetRequiredComponent<OrdersComponent>();
				var unitDesign = entity.GetRequiredComponent<OrbitalUnitDesignComponent>();
				var order = orders.GetActiveOrder<MovementOrderBase>();
				var newOrder = order.PrepareIntercept(entityLookup, position.GetCurrentAbsolutePosition(entityLookup), unitDesign.MaxSpeedPerTick, currentTime);
				if (newOrder != null)
					orders.UpdateOrder(newOrder);
			}

			foreach (var entity in entities)
			{
				var position = entity.GetRequiredComponent<EllipticalOrbitalPositionComponent>();

				if (entitiesWithMovementOrders.ContainsKey(entity.Id))
				{
					var orders = entity.GetRequiredComponent<OrdersComponent>();
					var order = orders.GetActiveOrder<MovementOrderBase>();
					var absolutePosition = position.GetCurrentAbsolutePosition(entityLookup);
					position.SetCurrentAbsolutePosition(order.MoveOneTick(absolutePosition));
				}
				else
				{
					var oldRelativePosition = position.RelativePosition;
					position.RelativePosition = position.GetRelativeOrbitalPositionAtTime(newTime);
					if (oldRelativePosition.DistanceTo(position.RelativePosition) > 1E11 && oldRelativePosition.X != 0.0)
						Log.Info($"Entity jumped from {oldRelativePosition.X},{oldRelativePosition.Y} to {position.RelativePosition.X},{position.RelativePosition.Y}");
				}
			}

			foreach (var entity in entitiesWithMovementOrders.Values)
			{
				var position = entity.GetRequiredComponent<EllipticalOrbitalPositionComponent>();
				var orders = entity.GetRequiredComponent<OrdersComponent>();
				var order = orders.GetActiveOrder<MovementOrderBase>();

				if (order.TryMarkAsResolved(entityLookup, position.GetCurrentAbsolutePosition(entityLookup), out var newOrder))
					orders.RemoveOrder(order);
				else if (newOrder != null)
					orders.UpdateOrder(newOrder);
				if (!orders.HasActiveOrder<MovementOrderBase>())
				{
					var text = string.Format(CultureInfo.InvariantCulture, OurResources.NotificationFinishedOrders, entity.Id);
					notificationLog.AddEvent(new Notification(text, newTime, true, entity.Id));
				}
			}
		}

		private static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(StarSystemMovementSystem));
	}
}