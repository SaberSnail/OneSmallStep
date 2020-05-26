using NUnit.Framework;
using OneSmallStep.ECS.Components;
using OneSmallStep.ECS.Templates;
using OneSmallStep.ECS.Utility;
using OneSmallStep.Utility;
using OneSmallStep.Utility.Math;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.Tests
{
	[TestFixture]
	public class PopulationTests
	{
		[OneTimeSetUp]
		public void Setup()
		{
			var lifetimeDistribution = new GompertzMakehamDistribution(1E-5, 0.085, 5.0E-3);
			var cohortTemplates = CohortTemplate.CreateAll(
				new CohortTemplateConfiguration
				{
					InfantsRequireExtraCare = true,
					PhysicalMaturation = 3,
					MentalMaturation = 4,
					LowFertilityStart = 7,
					InfertilityStart = 9,
					LifetimeDistribution = lifetimeDistribution,
				});
			var birthRate = 0.1 / Constants.DaysPerYear;
			m_racialTemplate = new RacialTemplate("human", "Human", lifetimeDistribution, birthRate, cohortTemplates);
		}

		[Test]
		public void PopulationDistributionTest()
		{
			var cohorts = new CohortCollection(m_racialTemplate);
			cohorts.DistributePopulationAcrossCohorts(1000000000);

			var stats = new PopulationChangeStats(new TimeOffset(0), 0, 0);

			var maxDate = new TimePoint((long) (Constants.DaysPerYear * 1));
			for (var date = new TimePoint(0); date < maxDate; date += TimeOffset.OneTick)
			{
				var newPops = PopulationUtility.ProcessTick(cohorts, out var tickStats);
				cohorts = cohorts.CloneWithNewPopulations(newPops);
				stats += tickStats;
			}
		}

		RacialTemplate m_racialTemplate;
	}
}
