using OneSmallStep.Time;
using OneSmallStep.Utility;

namespace OneSmallStep.ECS.Components
{
	public sealed class AgeTemplate
	{
		public AgeTemplate()
		{
			// Humans
			MeanDaysBetweenFailures = 10000 * Constants.TicksPerYear;
			AgeRiskDoublingDays = 8.0 * Constants.TicksPerYear;
		}
		public double MeanDaysBetweenFailures { get; }
		public double AgeRiskDoublingDays { get; }
	}
	public sealed class AgeComponent : ComponentBase
	{
		public AgeComponent(Entity entity, AgeTemplate template)
			: base(entity)
		{
			Template = template;
		}

		public TimePoint CreationDate { get; set; }

		public AgeTemplate Template { get; }

		public TimeOffset GetAge(TimePoint currentDate)
		{
			return CreationDate.GetTimeTo(currentDate);
		}
	}
}
