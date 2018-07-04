using OneSmallStep.ECS;
using OneSmallStep.ECS.Components;

namespace OneSmallStep.EntityViewModels
{
	public sealed class PlanetViewModel : EntityViewModelBase
	{
		public PlanetViewModel(Entity entity)
			: base(entity)
		{
		}

		public long Population
		{
			get
			{
				VerifyAccess();
				return m_population;
			}
			set
			{
				if (SetPropertyField(nameof(Population), value, ref m_population))
				{
					var populationComponent = Entity.GetComponent<PopulationComponent>();
					if (populationComponent != null)
						populationComponent.Population = m_population;
				}
			}
		}

		public override void UpdateFromEntity()
		{
			var populationComponent = Entity.GetComponent<PopulationComponent>();
			Population = populationComponent?.Population ?? 0;
		}

		long m_population;
	}
}
