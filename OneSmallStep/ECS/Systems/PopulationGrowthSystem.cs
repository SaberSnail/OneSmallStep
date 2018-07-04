using System;
using System.Collections;
using GoldenAnvil.Utility;
using OneSmallStep.ECS.Components;

namespace OneSmallStep.ECS.Systems
{
	public sealed class PopulationGrowthSystem : SystemBase
	{
		public PopulationGrowthSystem(GameData gameData, Random rng) : base(gameData)
		{
			m_rng = rng;
		}

		protected override BitArray GetComponentKey()
		{
			return GameData.EntityManager.CreateComponentKey(typeof(PopulationComponent));
		}

		protected override void ProcessTick(Entity entity)
		{
			PopulationComponent population = entity.GetComponent<PopulationComponent>();
			var growthRate = population.GrowthRate;
			growthRate = growthRate + growthRate * m_rng.NextGauss() * 0.5;
			population.Population = Math.Max(0, population.Population + (long) (population.Population * growthRate));
		}

		readonly Random m_rng;
	}
}
