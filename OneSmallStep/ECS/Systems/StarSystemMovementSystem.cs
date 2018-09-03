﻿using OneSmallStep.ECS.Components;
using OneSmallStep.Time;

namespace OneSmallStep.ECS.Systems
{
	public sealed class StarSystemMovementSystem : SystemBase
	{
		public override void ProcessTick(IEntityLookup entityLookup, NotificationLog eventLog, TimePoint newTime)
		{
			var entitiesList = entityLookup.GetEntitiesMatchingKey(GetRequiredComponentsKey(entityLookup));

			foreach (var entity in entitiesList)
			{
				var position = entity.GetRequiredComponent<OrbitalPositionComponent>();
				var orders = entity.GetOptionalComponent<MovementOrdersComponent>();
				var unitDesign = entity.GetOptionalComponent<OrbitalUnitDesignComponent>();

				if (!MovementOrderUtility.CanExecuteOrders(orders, unitDesign))
				{
					var body = entity.GetRequiredComponent<OrbitalBodyCharacteristicsComponent>();
					position.InitializeOrbitalValuesIfNeeded(entityLookup, body.Mass);
				}
			}

			foreach (var entity in entitiesList)
			{
				var position = entity.GetRequiredComponent<OrbitalPositionComponent>();
				var orders = entity.GetOptionalComponent<MovementOrdersComponent>();
				var unitDesign = entity.GetOptionalComponent<OrbitalUnitDesignComponent>();

				if (MovementOrderUtility.CanExecuteOrders(orders, unitDesign))
					orders.PrepareIntercept(entityLookup, position.GetCurrentAbsolutePosition(entityLookup), unitDesign.MaxSpeedPerTick);
			}

			foreach (var entity in entitiesList)
			{
				var position = entity.GetRequiredComponent<OrbitalPositionComponent>();
				var orders = entity.GetOptionalComponent<MovementOrdersComponent>();
				var unitDesign = entity.GetOptionalComponent<OrbitalUnitDesignComponent>();

				if (MovementOrderUtility.CanExecuteOrders(orders, unitDesign))
				{
					var absolutePosition = position.GetCurrentAbsolutePosition(entityLookup);
					position.SetCurrentAbsolutePosition(orders.MoveOneTick(absolutePosition));
				}
				else
				{
					position.RelativePosition = position.GetRelativeOrbitalPositionAtTime(new TimeOffset(1));
				}
			}

			foreach (var entity in entitiesList)
			{
				var position = entity.GetRequiredComponent<OrbitalPositionComponent>();
				var orders = entity.GetOptionalComponent<MovementOrdersComponent>();
				var unitDesign = entity.GetOptionalComponent<OrbitalUnitDesignComponent>();

				if (MovementOrderUtility.CanExecuteOrders(orders, unitDesign))
				{
					orders.ResolveOrderIfNeeded(entityLookup, position.GetCurrentAbsolutePosition(entityLookup));
					if (!orders.HasActiveOrder())
						eventLog.AddEvent(new Notification(OurResources.NotificationFinishedOrders, newTime, true, entity.Id));
				}
			}
		}

		protected override ComponentKey GetRequiredComponentsKey(IEntityLookup entityLookup)
		{
			return entityLookup.CreateComponentKey<OrbitalPositionComponent>();
		}
	}
}