using GoldenAnvil.Utility;
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

		public string Position
		{
			get
			{
				VerifyAccess();
				return m_position;
			}
			private set
			{
				SetPropertyField(nameof(Position), value, ref m_position);
			}
		}

		public override void UpdateFromEntity()
		{
			var population = Entity.GetComponent<PopulationComponent>();
			Population = population?.Population ?? 0;

			var astronomicalBody = Entity.GetComponent<AstronomicalBodyComponent>();
			var position = astronomicalBody?.GetAbsolutePosition();
			Position = position.HasValue ? "{0}, {1}".FormatCurrentUiCulture(position.Value.X, position.Value.Y) : null;
		}

		long m_population;
		string m_position;
	}
}
