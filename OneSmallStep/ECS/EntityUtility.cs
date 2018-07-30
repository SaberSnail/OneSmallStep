using System.Windows;
using OneSmallStep.ECS.Components;
using OneSmallStep.Time;

namespace OneSmallStep.ECS
{
	public static class EntityUtility
	{
		public static Entity CreatePlanet(this EntityManager entityManager, double mass, double radius)
		{
			var planet = new Entity(entityManager);
			planet.AddComponent(OrbitalPositionComponent.CreateUnpoweredBody());
			planet.AddComponent(new OrbitalBodyCharacteristicsComponent
			{
				Mass = mass,
				Radius = radius,
			});

			entityManager.RegisterEntity(planet);
			return planet;
		}

		public static Entity CreatePlanet(this EntityManager entityManager, Entity parent, double mass, double radius, double periodInDays, double meanAnomalyInDegrees, bool isPrograde, ICalendar calendar)
		{
			var planet = new Entity(entityManager);
			planet.AddComponent(OrbitalPositionComponent.CreateUnpoweredBody(parent, mass, periodInDays, meanAnomalyInDegrees, isPrograde, calendar));
			planet.AddComponent(new OrbitalBodyCharacteristicsComponent
			{
				Mass = mass,
				Radius = radius,
			});

			entityManager.RegisterEntity(planet);
			return planet;
		}

		public static Entity CreateShip(this EntityManager entityManager, Point startingPoint)
		{
			var ship = new Entity(entityManager);
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

			entityManager.RegisterEntity(ship);
			return ship;
		}
	}
}
