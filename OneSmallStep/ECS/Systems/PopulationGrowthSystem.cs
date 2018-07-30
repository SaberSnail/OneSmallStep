using System;
using System.Collections.Generic;
using System.Linq;
using GoldenAnvil.Utility;
using OneSmallStep.ECS.Components;
using OneSmallStep.Time;

namespace OneSmallStep.ECS.Systems
{
	public sealed class PopulationGrowthSystem : SystemBase
	{
		public PopulationGrowthSystem(GameData gameData, Random rng) : base(gameData)
		{
			m_rng = rng;
		}

		protected override ComponentKey GetRequiredComponentsKey()
		{
			return GameData.EntityManager.CreateComponentKey(typeof(PopulationComponent));
		}

		protected override void ProcessTickCore(TimePoint newTime, IEnumerable<Entity> entities)
		{
			var entitiesList = entities.ToList().AsReadOnly();

			foreach (var entity in entitiesList)
			{
				PopulationComponent population = entity.GetRequiredComponent<PopulationComponent>();
				var growthRate = population.GrowthRate;
				growthRate = growthRate + growthRate * m_rng.NextGauss() * 0.5;
				population.Population = Math.Max(0, population.Population + (long) (population.Population * growthRate));
			}
		}

		readonly Random m_rng;
	}
}
