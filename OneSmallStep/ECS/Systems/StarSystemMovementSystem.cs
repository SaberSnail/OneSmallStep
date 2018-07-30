using System.Collections.Generic;
using System.Linq;
using OneSmallStep.ECS.Components;
using OneSmallStep.Time;

namespace OneSmallStep.ECS.Systems
{
	public sealed class StarSystemMovementSystem : SystemBase
	{
		public StarSystemMovementSystem(GameData gameData)
			: base(gameData)
		{
		}

		protected override ComponentKey GetRequiredComponentsKey()
		{
			return GameData.EntityManager.CreateComponentKey<OrbitalPositionComponent>();
		}

		protected override void ProcessTickCore(TimePoint newTime, IEnumerable<Entity> entities)
		{
			var entitiesList = entities.ToList().AsReadOnly();

			foreach (var entity in entitiesList)
			{
				var position = entity.GetRequiredComponent<OrbitalPositionComponent>();
				var orders = entity.GetOptionalComponent<MovementOrdersComponent>();
				var unitDesign = entity.GetOptionalComponent<OrbitalUnitDesignComponent>();

				if (!MovementOrderUtility.CanExecuteOrders(orders, unitDesign))
				{
					var body = entity.GetRequiredComponent<OrbitalBodyCharacteristicsComponent>();
					position.InitializeOrbitalValuesIfNeeded(body.Mass);
				}
			}

			foreach (var entity in entitiesList)
			{
				var position = entity.GetRequiredComponent<OrbitalPositionComponent>();
				var orders = entity.GetOptionalComponent<MovementOrdersComponent>();
				var unitDesign = entity.GetOptionalComponent<OrbitalUnitDesignComponent>();

				if (MovementOrderUtility.CanExecuteOrders(orders, unitDesign))
					orders.PrepareIntercept(position.GetCurrentAbsolutePosition(), unitDesign.MaxSpeedPerTick);
			}

			foreach (var entity in entitiesList)
			{
				var position = entity.GetRequiredComponent<OrbitalPositionComponent>();
				var orders = entity.GetOptionalComponent<MovementOrdersComponent>();
				var unitDesign = entity.GetOptionalComponent<OrbitalUnitDesignComponent>();

				if (MovementOrderUtility.CanExecuteOrders(orders, unitDesign))
				{
					var absolutePosition = position.GetCurrentAbsolutePosition();
					position.SetCurrentAbsolutePosition(orders.MoveOneTick(absolutePosition));
				}
				else
				{
					var body = entity.GetRequiredComponent<OrbitalBodyCharacteristicsComponent>();
					position.RelativePosition = position.GetRelativeOrbitalPositionAtTime(new TimeOffset(1), body.Mass);
				}
			}

			foreach (var entity in entitiesList)
			{
				var position = entity.GetRequiredComponent<OrbitalPositionComponent>();
				var orders = entity.GetOptionalComponent<MovementOrdersComponent>();
				var unitDesign = entity.GetOptionalComponent<OrbitalUnitDesignComponent>();

				if (MovementOrderUtility.CanExecuteOrders(orders, unitDesign))
					orders.ResolveOrderIfNeeded(position.GetCurrentAbsolutePosition());
			}
		}
	}
}