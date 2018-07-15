using System.Windows;
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
			var body = OrbitalPositionComponent.CreateUnpoweredBody(mass);
			planet.AddComponent(body);

			entityManager.RegisterEntity(planet);
			return planet;
		}

		public static Entity CreatePlanet(this EntityManager entityManager, Entity parent, double mass, double periodInDays, double meanAnomalyInDegrees, ICalendar calendar)
		{
			var planet = new Entity(entityManager);
			var body = OrbitalPositionComponent.CreateUnpoweredBody(parent, mass, periodInDays, meanAnomalyInDegrees, calendar);
			planet.AddComponent(body);

			entityManager.RegisterEntity(planet);
			return planet;
		}

		public static Entity CreateShip(this EntityManager entityManager, Point startingPoint)
		{
			var ship = new Entity(entityManager);
			var body = OrbitalPositionComponent.CreatePoweredBody(5E10, 5E7, startingPoint);
			ship.AddComponent(body);

			entityManager.RegisterEntity(ship);
			return ship;
		}
	}
}
