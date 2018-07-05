using System;
using System.Collections;
using OneSmallStep.ECS.Components;
using OneSmallStep.Utility;

namespace OneSmallStep.ECS.Systems
{
	public sealed class AgeSystem : SystemBase
	{
		public AgeSystem(GameData gameData, Random rng)
			: base(gameData)
		{
			m_rng = rng;
		}

		protected override ComponentKey GetComponentKey()
		{
			return GameData.EntityManager.CreateComponentKey(typeof(AgeComponent));
		}

		protected override void ProcessTick(Entity entity)
		{
			AgeComponent ageComponent = entity.GetComponent<AgeComponent>();
			double survivalChance = 0.998;
				/*
				GetSurvivalChance(
				ageComponent.GetAge(GameData.CurrentDate).Days,
				ageComponent.Template.MeanDaysBetweenFailures,
				ageComponent.Template.AgeRiskDoublingDays);
				*/
			if (m_rng.NextDouble() > survivalChance)
			{
				// death
			}
		}

		private double GetSurvivalChance(double ageInTicks, double meanYearsBetweenFailures, double ageRiskDoublingYears)
		{
			double a = 1.0 / meanYearsBetweenFailures;
			const double b = 0.0001;
			double c = 1 + Constants.Ln2 / ageRiskDoublingYears;
			return Math.Exp(-a - ((b / Math.Log(c)) * Math.Pow(c, ageInTicks) * (c - 1.0)));
		}

		readonly Random m_rng;
	}
}
