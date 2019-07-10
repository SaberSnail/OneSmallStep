using System.Threading;
using OneSmallStep.ECS.Components;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS
{
	public sealed class GameData
	{
		public GameData(ICalendar calendar)
		{
			m_nextOrderId = 1;
			EntityManager = CreateEntityManager();
			Calendar = calendar;
		}

		public ICalendar Calendar { get; }
		public TimePoint CurrentDate { get; set; }
		public EntityManager EntityManager { get; }

		public OrderId GetNextOrderId()
		{
			return new OrderId(unchecked((uint) Interlocked.Increment(ref m_nextOrderId)));
		}

		private static EntityManager CreateEntityManager()
		{
			EntityManager entityManager = new EntityManager();

			using (entityManager.StartupScope())
			{
				entityManager.RegisterComponent<AgeComponent>();
				entityManager.RegisterComponent<EllipticalOrbitalPositionComponent>();
				entityManager.RegisterComponent<InformationComponent>();
				entityManager.RegisterComponent<OrbitalBodyCharacteristicsComponent>();
				entityManager.RegisterComponent<OrbitalUnitDesignComponent>();
				entityManager.RegisterComponent<OrdersComponent>();
				entityManager.RegisterComponent<PopulationComponent>();
				entityManager.RegisterComponent<ShipyardComponent>();
			}

			return entityManager;
		}

		int m_nextOrderId;
	}
}
