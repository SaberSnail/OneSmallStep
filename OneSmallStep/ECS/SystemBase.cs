using System.Collections.Generic;
using OneSmallStep.Time;

namespace OneSmallStep.ECS
{
	public abstract class SystemBase
	{
		public void ProcessTick(TimePoint newTime)
		{
			ProcessTickCore(newTime, GameData.EntityManager.GetEntitiesMatchingKey(GetRequiredComponentsKey()));
		}

		protected SystemBase(GameData gameData)
		{
			GameData = gameData;
		}

		protected GameData GameData { get; }

		protected abstract ComponentKey GetRequiredComponentsKey();

		protected abstract void ProcessTickCore(TimePoint newTime, IEnumerable<Entity> entities);
	}
}
