﻿using System;
using GoldenAnvil.Utility;
using OneSmallStep.ECS.Components;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS.Systems
{
	public sealed class PopulationGrowthSystem : SystemBase
	{
		public PopulationGrowthSystem()
		{
			m_rng = new Random();
		}

		public override void ProcessTick(IEntityLookup entityLookup, NotificationLog notificationLog, TimePoint newTime)
		{
			var entitiesList = entityLookup.GetEntitiesMatchingKey(entityLookup.CreateComponentKey(typeof(PopulationComponent)));

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
