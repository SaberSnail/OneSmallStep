using OneSmallStep.Utility.Math;

namespace OneSmallStep.ECS.Templates
{
	public sealed class CohortTemplateConfiguration
	{
		public bool InfantsRequireExtraCare { get; set; }
		public int PhysicalMaturation { get; set; }
		public int MentalMaturation { get; set; }
		public int LowFertilityStart { get; set; }
		public int InfertilityStart { get; set; }
		public ILifetimeDistribution LifetimeDistribution { get; set; }
	}
}
