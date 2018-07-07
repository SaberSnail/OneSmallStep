using OneSmallStep.ECS.Components;
using OneSmallStep.ECS.Systems;
using OneSmallStep.Time;

namespace OneSmallStep.ECS
{
	public static class EntityUtility
	{
		public static Entity CreatePlanet(this EntityManager entityManager, double mass)
		{
			var planet = new Entity(entityManager);
			var body = new AstronomicalBodyComponent
			{
				Mass = mass,
			};
			planet.AddComponent(body);

			entityManager.RegisterEntity(planet);
			return planet;
		}

		public static Entity CreatePlanet(this EntityManager entityManager, Entity parent, double mass, double periodInDays, double meanAnomalyInDegrees, ICalendar calendar)
		{
			var planet = new Entity(entityManager);
			var body = OrbitalDynamicsSystem.CreateComponent(parent, mass, periodInDays, meanAnomalyInDegrees, calendar);
			planet.AddComponent(body);

			entityManager.RegisterEntity(planet);
			return planet;
		}
	}
}
