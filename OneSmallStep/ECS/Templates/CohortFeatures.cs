using System;

namespace OneSmallStep.ECS.Templates
{
	[Flags]
	public enum CohortFeatures
	{
		None = 0,
		All = ~None,

		RequiresCare = 1 << 0,
		RequiresExtraCare = 1 << 1,
		Fertile = 1 << 2,
		LowFertile = 1 << 3,
		Student = 1 << 4,
		Worker = 1 << 5,
	}
}
