using System;
using System.Collections.Generic;
using GoldenAnvil.Utility;
using OneSmallStep.Utility.Math;

namespace OneSmallStep.ECS.Templates
{
	public sealed class RacialTemplate
	{
		public RacialTemplate(string id, string name, ILifetimeDistribution lifetimeDistribution, double birthRatePerTick, IEnumerable<CohortTemplate> cohortTemplates)
		{
			Id = id;
			Name = name;
			BirthRatePerTick = birthRatePerTick;
			CohortTemplates = cohortTemplates.AsReadOnlyList();
			m_lifetimeDistribution = lifetimeDistribution;
		}

		public string Id { get; }

		public string Name { get; }

		public double BirthRatePerTick { get; }

		public IReadOnlyList<CohortTemplate> CohortTemplates { get; }

		public double GetAverageDeathRatePerTick(int cohortIndex)
		{
			var start = CohortTemplates[cohortIndex].StartAge;
			var pastEnd = CohortTemplates[cohortIndex].PastEndAge;

			var probabilityOfLiving = m_lifetimeDistribution.ConditionalReliability(start, pastEnd - start);
			var probabilityOfDyingPerTick = 1.0 - Math.Pow(probabilityOfLiving, 1.0 / (double) (pastEnd - start).TickOffset);

			return probabilityOfDyingPerTick;
		}

		ILifetimeDistribution m_lifetimeDistribution;
	}
}
