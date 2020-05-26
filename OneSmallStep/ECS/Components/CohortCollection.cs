using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GoldenAnvil.Utility;
using OneSmallStep.ECS.Templates;
using OneSmallStep.ECS.Utility;
using OneSmallStep.Utility.Time;
using static System.Math;

namespace OneSmallStep.ECS.Components
{
	public sealed class CohortCollection
	{
		public CohortCollection(RacialTemplate racialTemplate)
			: this(racialTemplate, racialTemplate.CohortTemplates.Select(x => new CohortImpl(x, 0L)).ToArray(), new Lazy<IReadOnlyList<double>>(() => GetDistributionRatios(racialTemplate)))
		{
		}

		private CohortCollection(RacialTemplate racialTemplate, IEnumerable<long> populations)
			: this(racialTemplate, populations.Select((pop, i) => new CohortImpl(racialTemplate.CohortTemplates[i], pop)).ToArray(), new Lazy<IReadOnlyList<double>>(() => GetDistributionRatios(racialTemplate)))
		{
		}

		public CohortCollection Clone() => new CohortCollection(this);

		public CohortCollection CloneWithNewPopulations(IEnumerable<long> populations) => new CohortCollection(this, populations);

		public RacialTemplate RacialTemplate { get; }

		public IReadOnlyList<ICohort> Cohorts => m_cohorts;

		public long TotalPopulation => m_cohorts.Sum(x => x.Population);

		public long GetPopulationWithAnyFeature(CohortFeatures features) => m_cohorts.Where(x => (x.Features & features) != CohortFeatures.None).Sum(x => x.Population);

		public long GetPopulationWithAllFeatures(CohortFeatures features) => m_cohorts.Where(x => (x.Features & features) == features).Sum(x => x.Population);

		public void DistributePopulationAcrossCohorts(long populationToAdd)
		{
			var distributionRatios = m_distributionRatios.Value;

			var totalAddedPopulation = 0L;
			for (var i = m_cohorts.Length - 1; i >= 0; i--)
			{
				var additionalPopulation = (long) Round(populationToAdd * distributionRatios[i]);
				totalAddedPopulation += additionalPopulation;
				m_cohorts[i].Population += additionalPopulation;
			}

			var excess = populationToAdd - totalAddedPopulation;
			if (excess != 0)
			{
				var increment = excess > 0 ? 1 : -1;
				var index = excess > 0 ? 0 : m_cohorts.Length - 1;
				while (excess != 0)
				{
					m_cohorts[index].Population += increment;
					excess -= increment;
				}
			}
		}

		private CohortCollection(CohortCollection that)
			: this(that.RacialTemplate, that.m_cohorts.Select(x => x.Clone()).ToArray(), that.m_distributionRatios)
		{
		}

		private CohortCollection(CohortCollection that, IEnumerable<long> populations)
			: this(that.RacialTemplate, populations.Select((pop, i) => new CohortImpl(that.RacialTemplate.CohortTemplates[i], pop)).ToArray(), that.m_distributionRatios)
		{
		}

		private CohortCollection(RacialTemplate racialTemplate, CohortImpl[] cohorts, Lazy<IReadOnlyList<double>> distributionRatios)
		{
			RacialTemplate = racialTemplate;
			m_cohorts = cohorts;
			m_distributionRatios = distributionRatios;
		}

		/// <summary>
		/// Simulate a test population until all original people are deceased and use the
		/// final population distribution as a basis for distributing newly created populations.
		/// </summary>
		private static IReadOnlyList<double> GetDistributionRatios(RacialTemplate racialTemplate)
		{
			long testPopulation = 1000000;
			var ageRange = (double) racialTemplate.CohortTemplates.Sum(x => (x.PastEndAge - x.StartAge).TickOffset);
			var pops = racialTemplate.CohortTemplates
				.Select(cohortTemplate =>
				{
					var cohortAgeRange = (double) (cohortTemplate.PastEndAge - cohortTemplate.StartAge).TickOffset;
					return (long) ((double) testPopulation * cohortAgeRange / ageRange);
				});
			var testCohorts = new CohortCollection(racialTemplate, pops);

			var maxDate = new TimePoint(0) + testCohorts.Cohorts.Last().PastEnd;
			for (var date = new TimePoint(0); date < maxDate; date += TimeOffset.OneTick)
				testCohorts = new CohortCollection(racialTemplate, PopulationUtility.ProcessTick(testCohorts, out var _));

			var finalTestPopulation = (double) testCohorts.TotalPopulation;
			return testCohorts.Cohorts
				.Select(x => (double) x.Population / finalTestPopulation)
				.AsReadOnlyList();
		}

		[DebuggerDisplay("{Start.TickOffset / 365.2425} : {Population}")]
		private sealed class CohortImpl : ICohort
		{
			public CohortImpl(CohortTemplate template, long population)
			{
				m_template = template;
				Population = population;
			}

			private CohortImpl(CohortImpl that)
			{
				m_template = that.m_template;
				Population = that.Population;
			}

			public TimeOffset Start => m_template.StartAge;
			public TimeOffset PastEnd => m_template.PastEndAge;
			public long Population { get; set; }
			public CohortFeatures Features => m_template.Features;

			public CohortImpl Clone() => new CohortImpl(this);

			readonly CohortTemplate m_template;
		}

		CohortImpl[] m_cohorts;
		Lazy<IReadOnlyList<double>> m_distributionRatios;
	}
}
