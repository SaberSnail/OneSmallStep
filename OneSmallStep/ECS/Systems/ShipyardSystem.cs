using System;
using OneSmallStep.ECS.Components;
using OneSmallStep.Utility;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS.Systems
{
	public sealed class ShipyardSystem : SystemBase
	{
		public override void ProcessTick(IEntityLookup entityLookup, NotificationLog notificationLog, TimePoint newTime)
		{
			var entities = entityLookup.GetEntitiesMatchingKey(entityLookup.CreateComponentKey<ShipyardComponent>());
			foreach (var entity in entities)
			{
				if (((newTime.Tick - 1) % (int) Math.Round(Constants.TicksPerDay * 365.25 * 5)) == 0)
				{
					var position = entity.GetRequiredComponent<EllipticalOrbitalPositionComponent>().GetCurrentAbsolutePosition(entityLookup);
					EntityUtility.CreateShip(entityLookup, "Discovery", position);
				}
			}
		}
	}
}
