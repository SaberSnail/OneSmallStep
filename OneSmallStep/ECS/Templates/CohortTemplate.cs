using System;
using System.Collections.Generic;
using OneSmallStep.Utility;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS.Templates
{
	public sealed class CohortTemplate
	{
		public const long CohortLengthInDays = (long) (Constants.DaysPerYear * 5);
		public const double CareRequiredThreshold = 0.1;
		public const double ExtraCareRequiredThreshold = 0.3;
		public const int MaxCohortIndex = 100;

		public static CohortTemplate[] CreateAll(CohortTemplateConfiguration config)
		{
			if (config.LowFertilityStart < config.PhysicalMaturation)
				throw new ArgumentException($"{nameof(CohortTemplateConfiguration.LowFertilityStart)} must be after {nameof(CohortTemplateConfiguration.PhysicalMaturation)}.", nameof(config));
			if (config.InfertilityStart <= config.PhysicalMaturation)
				throw new ArgumentException($"{nameof(CohortTemplateConfiguration.InfertilityStart)} must be after {nameof(CohortTemplateConfiguration.PhysicalMaturation)}.", nameof(config));
			if (config.LowFertilityStart > config.InfertilityStart)
				throw new ArgumentException($"{nameof(CohortTemplateConfiguration.InfertilityStart)} must be at or after {nameof(CohortTemplateConfiguration.LowFertilityStart)}.", nameof(config));
			if (config.LifetimeDistribution is null)
				throw new ArgumentException($"{nameof(CohortTemplateConfiguration.LifetimeDistribution)} must be specified.", nameof(config));

			var templates = new List<CohortTemplate>();
			int index = 0;
			while (true)
			{
				var cohortStartAge = new TimeOffset(CohortLengthInDays * index);
				var cohortPastEndAge = new TimeOffset(CohortLengthInDays * (index + 1));
				var startFailureRate = config.LifetimeDistribution.FailureRate(cohortStartAge);

				var features = CohortFeatures.None;
				if (config.InfantsRequireExtraCare && index == 0)
				{
					features |= CohortFeatures.RequiresExtraCare;
				}
				else
				{
					if (index < config.MentalMaturation)
						features |= CohortFeatures.Student;

					if (index < config.PhysicalMaturation)
						features |= CohortFeatures.RequiresCare;
					else if (index < config.LowFertilityStart)
						features |= CohortFeatures.Fertile;
					else if (index < config.InfertilityStart)
						features |= CohortFeatures.LowFertile;

					if (startFailureRate >= ExtraCareRequiredThreshold)
						features |= CohortFeatures.RequiresExtraCare;
					else if (startFailureRate >= CareRequiredThreshold)
						features |= CohortFeatures.RequiresCare;
				}

				templates.Add(new CohortTemplate(features, cohortStartAge, cohortPastEndAge));

				if (startFailureRate >= 1.0)
					break;
				index++;
				if (index > MaxCohortIndex)
					throw new InvalidOperationException($"Cohort index exceeded {MaxCohortIndex}.");
			}

			return templates.ToArray();
		}

		public CohortTemplate(CohortFeatures features, TimeOffset startAge, TimeOffset pastEndAge)
		{
			Features = features;
			StartAge = startAge;
			PastEndAge = pastEndAge;
		}

		public CohortFeatures Features { get; }

		public TimeOffset StartAge { get; }

		public TimeOffset PastEndAge { get; }
	}
}
