using OneSmallStep.Time;
using OneSmallStep.Utility;

namespace OneSmallStep.ECS.Components
{
	public sealed class AgeTemplate
	{
		public AgeTemplate()
		{
			// Humans
			MeanDaysBetweenFailures = 10000 * 365 * Constants.TicksPerDay;
			AgeRiskDoublingDays = 8.0 * 365 * Constants.TicksPerDay;
		}
		public double MeanDaysBetweenFailures { get; }
		public double AgeRiskDoublingDays { get; }
	}
	public sealed class AgeComponent : ComponentBase
	{
		public AgeComponent(AgeTemplate template)
		{
			Template = template;
		}

		public TimePoint CreationDate
		{
			get => m_creationDate;
			set => SetPropertyField(value, ref m_creationDate);
		}

		public AgeTemplate Template { get; }

		public TimeOffset GetAge(TimePoint currentDate)
		{
			return CreationDate.GetTimeTo(currentDate);
		}

		public override ComponentBase Clone()
		{
			return new AgeComponent(this);
		}

		private AgeComponent(AgeComponent that)
		: base(that)
		{
			Template = that.Template;
			m_creationDate = that.m_creationDate;
		}

		TimePoint m_creationDate;
	}
}
