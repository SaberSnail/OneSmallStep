using System;
using OneSmallStep.Utility;

namespace OneSmallStep.ECS.Components
{
	public sealed class PopulationComponent : ComponentBase
	{
		public PopulationComponent()
		{
			GrowthRate = Math.Pow(1 + c_averageGrowthRate, 1.0 / (Constants.TicksPerDay * 365)) - 1;
		}

		public double GrowthRate { get; }

		public long Population
		{
			get => m_population;
			set => SetPropertyField(value, ref m_population);
		}

		public override ComponentBase Clone()
		{
			return new PopulationComponent(this);
		}

		private PopulationComponent(PopulationComponent that)
			: base(that)
		{
			m_population = that.m_population;
		}

		const double c_averageGrowthRate = 0.01;

		long m_population;
	}
}
