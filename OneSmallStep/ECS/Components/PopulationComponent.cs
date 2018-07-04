using System;
using OneSmallStep.Utility;

namespace OneSmallStep.ECS.Components
{
	public sealed class PopulationComponent : ComponentBase
	{
		public PopulationComponent(Entity entity, Random rng) : base(entity)
		{
			GrowthRate = Math.Pow(1 + c_averageGrowthRate, 1.0 / (Constants.TicksPerDay * 365)) - 1;
		}

		public double GrowthRate { get; }

		public long Population { get; set; }

		const double c_averageGrowthRate = 0.01;
	}
}
