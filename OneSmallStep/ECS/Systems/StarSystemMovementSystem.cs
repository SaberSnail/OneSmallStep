using System.ComponentModel;
using System.Globalization;
using System.Linq;
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
			var entities = entityLookup.GetEntitiesMatchingKey(GetRequiredComponentsKey(entityLookup));

			foreach (var entity in entities)
			{
				var position = entity.GetRequiredComponent<EllipticalOrbitalPositionComponent>();
				var orders = entity.GetOptionalComponent<MovementOrdersComponent>();
				var unitDesign = entity.GetOptionalComponent<OrbitalUnitDesignComponent>();

				if (!MovementOrderUtility.CanExecuteOrders(orders, unitDesign))
				{
					var body = entity.GetRequiredComponent<OrbitalBodyCharacteristicsComponent>();
					position.InitializeOrbitalValuesIfNeeded(entityLookup, body.Mass);
				}
			}

			foreach (var entity in entities)
			{
				var position = entity.GetRequiredComponent<EllipticalOrbitalPositionComponent>();
				var orders = entity.GetOptionalComponent<MovementOrdersComponent>();
				var unitDesign = entity.GetOptionalComponent<OrbitalUnitDesignComponent>();

				if (MovementOrderUtility.CanExecuteOrders(orders, unitDesign))
				{
					var currentTime = newTime - new TimeOffset(1);
					orders.PrepareIntercept(entityLookup, position.GetCurrentAbsolutePosition(entityLookup), unitDesign.MaxSpeedPerTick, currentTime);
				}
			}

			foreach (var entity in entities)
			{
				var position = entity.GetRequiredComponent<EllipticalOrbitalPositionComponent>();
				var orders = entity.GetOptionalComponent<MovementOrdersComponent>();
				var unitDesign = entity.GetOptionalComponent<OrbitalUnitDesignComponent>();

				if (MovementOrderUtility.CanExecuteOrders(orders, unitDesign))
				{
					var absolutePosition = position.GetCurrentAbsolutePosition(entityLookup);
					position.SetCurrentAbsolutePosition(orders.MoveOneTick(absolutePosition));
				}
				else
				{
					var oldRelativePosition = position.RelativePosition;
					position.RelativePosition = position.GetRelativeOrbitalPositionAtTime(newTime);
					if (oldRelativePosition.DistanceTo(position.RelativePosition) > 1E11 && oldRelativePosition.X != 0.0)
					{
						Log.Info($"Entity jumped from {oldRelativePosition.X},{oldRelativePosition.Y} to {position.RelativePosition.X},{position.RelativePosition.Y}");
					}
				}
			}

			foreach (var entity in entities)
			{
				var position = entity.GetRequiredComponent<EllipticalOrbitalPositionComponent>();
				var orders = entity.GetOptionalComponent<MovementOrdersComponent>();
				var unitDesign = entity.GetOptionalComponent<OrbitalUnitDesignComponent>();

				if (MovementOrderUtility.CanExecuteOrders(orders, unitDesign))
				{
					orders.ResolveOrderIfNeeded(entityLookup, position.GetCurrentAbsolutePosition(entityLookup));
					if (!orders.HasActiveOrder())
					{
						var text = string.Format(CultureInfo.InvariantCulture, OurResources.NotificationFinishedOrders, entity.Id);
						notificationLog.AddEvent(new Notification(text, newTime, true, entity.Id));
					}
				}
			}
		}

		protected override ComponentKey GetRequiredComponentsKey(IEntityLookup entityLookup)
		{
			return entityLookup.CreateComponentKey<EllipticalOrbitalPositionComponent>();
		}

		private static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(StarSystemMovementSystem));
	}
}