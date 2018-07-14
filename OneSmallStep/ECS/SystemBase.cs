using System.Collections.Generic;

namespace OneSmallStep.ECS
{
	public abstract class SystemBase
	{
		public void Process()
		{
			ProcessTick(GameData.EntityManager.GetEntitiesMatchingKey(GetRequiredComponentsKey()));
		}

		protected SystemBase(GameData gameData)
		{
			GameData = gameData;
		}

		protected GameData GameData { get; }

		protected abstract ComponentKey GetRequiredComponentsKey();

		protected abstract void ProcessTick(IEnumerable<Entity> entities);
	}
}
