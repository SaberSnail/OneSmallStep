using OneSmallStep.Utility.Time;

namespace OneSmallStep.Utility.Math
{
	public interface ILifetimeDistribution
	{
		/// <summary>
		/// The PDF, also called f(t).
		/// </summary>
		double Pdf(TimeOffset t);

		/// <summary>
		/// The CDF, also called F(t), Q(t), unreliability function, or probability of failure.
		/// 1.0 - Reliability(t);
		/// </summary>
		double Cdf(TimeOffset t);

		/// <summary>
		/// The Reliability function, also called R(t).
		/// </summary>
		double Reliability(TimeOffset t);

		/// <summary>
		/// The Conditional Reliability, also called R(t|T).
		/// The chance of survival for time t, given having already survived to an age.
		/// Reliability(age + t) / Reliability(age)
		/// </summary>
		double ConditionalReliability(TimeOffset age, TimeOffset t);

		/// <summary>
		/// The failure rate function, also called lambda(t), hazard rate, or instantaneous failure rate.
		/// Pdf(t) / Reliability(t);
		/// </summary>
		double FailureRate(TimeOffset t);
	}
}
