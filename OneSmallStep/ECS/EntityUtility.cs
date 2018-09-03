using System.Windows;
using OneSmallStep.ECS.Components;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS
{
	public static class EntityUtility
	{
		public static Entity CreatePlanet(IEntityLookup entityLookup, double mass, double radius)
		{
			var planet = new Entity(entityLookup);
			planet.AddComponent(OrbitalPositionComponent.CreateUnpoweredBody());
			planet.AddComponent(new OrbitalBodyCharacteristicsComponent
			{
				Mass = mass,
				Radius = radius,
			});

			entityLookup.RegisterEntity(planet);
			return planet;
		}

		public static Entity CreatePlanet(IEntityLookup entityLookup, Entity parent, double mass, double radius, double periodInDays, double meanAnomalyInDegrees, bool isPrograde, ICalendar calendar)
		{
			var planet = new Entity(entityLookup);
			planet.AddComponent(OrbitalPositionComponent.CreateUnpoweredBody(parent, mass, periodInDays, meanAnomalyInDegrees, isPrograde, calendar));
			planet.AddComponent(new OrbitalBodyCharacteristicsComponent
			{
				Mass = mass,
				Radius = radius,
			});

			entityLookup.RegisterEntity(planet);
			return planet;
		}

		public static Entity CreateShip(IEntityLookup entityLookup, Point startingPoint)
		{
			var ship = new Entity(entityLookup);
			ship.AddComponent(OrbitalPositionComponent.CreatePoweredBody(startingPoint));
			ship.AddComponent(new OrbitalBodyCharacteristicsComponent
			{
				Mass = 5E7,
				Radius = 200,
			});
			ship.AddComponent(new OrbitalUnitDesignComponent
			{
				MaxSpeedPerTick = 5E10,
			});
			ship.AddComponent(new MovementOrdersComponent());

			entityLookup.RegisterEntity(ship);
			return ship;
		}
	}
}
