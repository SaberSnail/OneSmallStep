using System;
using OneSmallStep.ECS.Components;

namespace OneSmallStep.ECS
{
	public static class EntityUtility
	{
		public static Entity CreatePlanet(this EntityManager entityManager, Random rng)
		{
			var planet = new Entity(entityManager);

			var population = new PopulationComponent(planet, rng) { Population = 1000000 };
			planet.AddComponent(population);

			entityManager.RegisterEntity(planet);
			return planet;
		}
	}
}
