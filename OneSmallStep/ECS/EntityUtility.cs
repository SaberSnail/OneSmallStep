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
			var body = OrbitalPositionComponent.CreateUnpoweredBody(mass, radius);
			planet.AddComponent(body);

			entityManager.RegisterEntity(planet);
			return planet;
		}

		public static Entity CreatePlanet(this EntityManager entityManager, Entity parent, double mass, double radius, double periodInDays, double meanAnomalyInDegrees, bool isPrograde, ICalendar calendar)
		{
			var planet = new Entity(entityManager);
			var body = OrbitalPositionComponent.CreateUnpoweredBody(parent, mass, radius, periodInDays, meanAnomalyInDegrees, isPrograde, calendar);
			planet.AddComponent(body);

			entityManager.RegisterEntity(planet);
			return planet;
		}

		public static Entity CreateShip(this EntityManager entityManager, Point startingPoint)
		{
			var ship = new Entity(entityManager);
			var body = OrbitalPositionComponent.CreatePoweredBody(5E10, 5E7, 200, startingPoint);
			ship.AddComponent(body);

			entityManager.RegisterEntity(ship);
			return ship;
		}
	}
}
