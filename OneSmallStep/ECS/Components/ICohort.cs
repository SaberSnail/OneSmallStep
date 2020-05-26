using OneSmallStep.ECS.Templates;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS.Components
{
	public interface ICohort
	{
		TimeOffset Start { get; }
		TimeOffset PastEnd { get; }
		long Population { get; }
		CohortFeatures Features { get; }
	}
}
