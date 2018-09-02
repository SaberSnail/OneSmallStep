﻿using OneSmallStep.ECS.Components;
using OneSmallStep.Time;

namespace OneSmallStep.ECS
{
	public sealed class GameData
	{
		public GameData(ICalendar calendar)
		{
			EntityManager = CreateEntityManager();
			Calendar = calendar;
		}

		public ICalendar Calendar { get; }
		public TimePoint CurrentDate { get; set; }
		public EntityManager EntityManager { get; }

		private static EntityManager CreateEntityManager()
		{
			EntityManager entityManager = new EntityManager();

			using (entityManager.StartupScope())
			{
				entityManager.RegisterComponent<AgeComponent>();
				entityManager.RegisterComponent<PopulationComponent>();
				entityManager.RegisterComponent<OrbitalPositionComponent>();
				entityManager.RegisterComponent<OrbitalBodyCharacteristicsComponent>();
				entityManager.RegisterComponent<OrbitalUnitDesignComponent>();
				entityManager.RegisterComponent<MovementOrdersComponent>();
			}

			return entityManager;
		}
	}
}
