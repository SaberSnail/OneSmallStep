using System;
using OneSmallStep.ECS.Components;
using OneSmallStep.ECS.Utility;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS.Systems
{
	public sealed class PopulationGrowthSystem : SystemBase
	{
		public PopulationGrowthSystem()
		{
		}

		public override void ProcessTick(IEntityLookup entityLookup, NotificationLog notificationLog, TimePoint newTime)
		{
			var entitiesList = entityLookup.GetEntitiesMatchingKey(entityLookup.CreateComponentKey(typeof(PopulationComponent)));

			foreach (var entity in entitiesList)
			{
				var component = entity.GetRequiredComponent<PopulationComponent>();
				var populations = component.Populations;
				foreach (var population in populations)
				{
					var newPops = PopulationUtility.ProcessTick(population, out var _);
					component.SetPopulation(population.RacialTemplate, newPops);
				}
			}
		}
	}
}
