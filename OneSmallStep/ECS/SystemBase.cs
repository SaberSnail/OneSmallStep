using System.Collections;

namespace OneSmallStep.ECS
{
	public abstract class SystemBase
	{
		public void Process()
		{
			foreach (Entity entity in GameData.EntityManager.GetEntitiesMatchingKey(GetRequiredComponentsKey()))
				ProcessTick(entity);
		}

		protected SystemBase(GameData gameData)
		{
			GameData = gameData;
		}

		protected GameData GameData { get; }

		protected abstract ComponentKey GetRequiredComponentsKey();

		protected abstract void ProcessTick(Entity entity);
	}
}
