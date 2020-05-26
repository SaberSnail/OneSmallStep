
using System.Collections.Generic;
using OneSmallStep.ECS.Components;
using OneSmallStep.ECS.Templates;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS.Utility
{
	public sealed class PopulationChangeStats
	{
		public PopulationChangeStats(TimeOffset timeSpan, long deaths, long births)
		{
			TimeSpan = timeSpan;
			Deaths = deaths;
			Births = births;
		}

		TimeOffset TimeSpan { get; }
		long Deaths { get; }
		long Births { get; }

		public static PopulationChangeStats operator +(PopulationChangeStats left, PopulationChangeStats right)
		{
			return new PopulationChangeStats(
				left.TimeSpan + right.TimeSpan,
				left.Deaths + right.Deaths,
				left.Births + right.Births);
		}
	}

	public static class PopulationUtility
	{
		public static IEnumerable<long> ProcessTick(CohortCollection cohorts, out PopulationChangeStats stats)
		{
			var newPops = new long[cohorts.Cohorts.Count];

			var totalDeaths = 0L;

			var racialTemplate = cohorts.RacialTemplate;
			var fertileFemales =
				(cohorts.GetPopulationWithAllFeatures(CohortFeatures.Fertile) / 2) +
				(cohorts.GetPopulationWithAllFeatures(CohortFeatures.LowFertile) / 4);
			var births = (long) (racialTemplate.BirthRatePerTick * fertileFemales);

			var graduatingCount = births;
			for (int i = 0; i < racialTemplate.CohortTemplates.Count; i++)
			{
				var cohortSize = cohorts.Cohorts[i].Population;
				var deathCount = (long) (cohorts.RacialTemplate.GetAverageDeathRatePerTick(i) * cohortSize);
				totalDeaths += deathCount;
				var newCohortSize = cohortSize - deathCount + graduatingCount;

				var template = racialTemplate.CohortTemplates[i];
				graduatingCount = cohortSize / (template.PastEndAge - template.StartAge).TickOffset;
				newCohortSize -= graduatingCount;

				newPops[i] = newCohortSize;
			}
			totalDeaths += graduatingCount;

			stats = new PopulationChangeStats(TimeOffset.OneTick, totalDeaths, births);

			return newPops;
		}
	}
}
