using System.Windows;
using OneSmallStep.ECS.Components;

namespace OneSmallStep.ECS
{
	public static class EntityUtility
	{
		public static Entity CreatePlanet(IEntityLookup entityLookup, string name, double mass, double radius)
		{
			var planet = new Entity(entityLookup);
			planet.AddComponent(new InformationComponent { Name = name });
			planet.AddComponent(EllipticalOrbitalPositionComponent.CreateUnpoweredBody());
			planet.AddComponent(new OrbitalBodyCharacteristicsComponent
			{
				Mass = mass,
				Radius = radius,
			});

			entityLookup.RegisterEntity(planet);
			return planet;
		}

		public static Entity CreatePlanet(IEntityLookup entityLookup, string name, EntityId parentId, double mass, double radius, double semiMajorAxis, double eccentricity, double longitudeOfPeriapsis, double meanAnomaly, bool isRetrograde, int epoch)
		{
			var planet = new Entity(entityLookup);
			planet.AddComponent(new InformationComponent { Name = name });

			// TODO: calculate correct initial position usinig epoch

			planet.AddComponent(EllipticalOrbitalPositionComponent.CreateUnpoweredBody(parentId, semiMajorAxis, eccentricity, longitudeOfPeriapsis, meanAnomaly, isRetrograde));
			planet.AddComponent(new OrbitalBodyCharacteristicsComponent
			{
				Mass = mass,
				Radius = radius,
			});

			entityLookup.RegisterEntity(planet);
			return planet;
		}

		public static Entity CreateShip(IEntityLookup entityLookup, string name, Point startingPoint)
		{
			var ship = new Entity(entityLookup);
			ship.AddComponent(new InformationComponent { Name = name });
			ship.AddComponent(EllipticalOrbitalPositionComponent.CreatePoweredBody(startingPoint));
			ship.AddComponent(new OrbitalBodyCharacteristicsComponent
			{
				Mass = 5E7,
				Radius = 200,
			});
			ship.AddComponent(new OrbitalUnitDesignComponent
			{
				MaxSpeedPerTick = 5E10,
			});
			ship.AddComponent(new OrdersComponent());

			entityLookup.RegisterEntity(ship);
			return ship;
		}

		public static void MakeHomeWorld(Entity entity)
		{
			var population = new PopulationComponent { Population = 1000000000 };
			entity.AddComponent(population);

			var shipyard = new ShipyardComponent();
			entity.AddComponent(shipyard);
		}
	}
}
