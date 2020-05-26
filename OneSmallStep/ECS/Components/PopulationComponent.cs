using System.Collections.Generic;
using System.Linq;
using GoldenAnvil.Utility;
using OneSmallStep.ECS.Templates;

namespace OneSmallStep.ECS.Components
{
	public sealed class PopulationComponent : ComponentBase
	{
		public PopulationComponent()
		{
			m_populations = new Dictionary<string, CohortCollection>();
		}

		public IReadOnlyList<CohortCollection> Populations => m_populations.Values.AsReadOnlyList();

		public void AddPopulation(RacialTemplate racialTemplate, long count)
		{
			using (ScopedPropertyChange())
			{
				var cohortCollection = m_populations.GetOrAddValue(racialTemplate.Id, new CohortCollection(racialTemplate));
				cohortCollection.DistributePopulationAcrossCohorts(count);
			}
		}

		public void SetPopulation(RacialTemplate racialTemplate, IEnumerable<long> populations)
		{
			using (ScopedPropertyChange())
			{
				var cohortCollection = m_populations.GetOrAddValue(racialTemplate.Id, new CohortCollection(racialTemplate));
				m_populations[racialTemplate.Id] = cohortCollection.CloneWithNewPopulations(populations);
			}
		}

		public override ComponentBase Clone()
		{
			return new PopulationComponent(this);
		}

		private PopulationComponent(PopulationComponent that)
			: base(that)
		{
			m_populations = that.m_populations
				.Select(x => x.Value.Clone())
				.ToDictionary(x => x.RacialTemplate.Id);
		}

		Dictionary<string, CohortCollection> m_populations;
	}
}
