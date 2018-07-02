using System.Collections;

namespace OneSmallStep.ECS
{
	public abstract class SystemBase
	{
		public void Process()
		{
			foreach (Entity entity in m_entityManager.GetEntitiesMatchingKey(GetComponentKey()))
				ProcessTick(entity);
		}

		protected SystemBase(GameData gameData, EntityManager entityManager)
		{
			m_gameData = gameData;
			m_entityManager = entityManager;
		}

		protected GameData GameData
		{
			get { return m_gameData; }
		}

		protected EntityManager EntityManager
		{
			get { return m_entityManager; }
		}

		protected abstract BitArray GetComponentKey();

		protected abstract void ProcessTick(Entity entity);

		readonly EntityManager m_entityManager;
		readonly GameData m_gameData;
	}
}
