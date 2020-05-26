using OneSmallStep.Utility.Time;
using static System.Math;

namespace OneSmallStep.Utility.Math
{
	public sealed class GompertzMakehamDistribution : ILifetimeDistribution
	{
		public GompertzMakehamDistribution(double alpha, double beta, double ageIndependentFactor)
		{
			m_alpha = alpha;
			m_beta = beta;
			m_lambda = ageIndependentFactor;
		}

		public double Pdf(TimeOffset t)
		{
			var time = (double) t.TickOffset / 365.2425;
			var eBetaTime = Exp(m_beta * time);
			return (m_lambda + (m_alpha * eBetaTime)) * Exp((-m_lambda * time) - (m_alpha * (eBetaTime - 1.0) / m_beta));
		}

		public double Cdf(TimeOffset t)
		{
			var time = (double) t.TickOffset / 365.2425;
			return 1.0 - Exp((-m_lambda * time) - (m_alpha * (Exp(m_beta * time) - 1.0) / m_beta));
		}

		public double Reliability(TimeOffset t)
		{
			var time = (double) t.TickOffset / 365.2425;
			return Exp((-m_lambda * time) - (m_alpha * (Exp(m_beta * time) - 1.0) / m_beta));
		}

		public double ConditionalReliability(TimeOffset age, TimeOffset t)
		{
			return Reliability(age + t) / Reliability(age);
		}

		public double FailureRate(TimeOffset t)
		{
			var time = (double) t.TickOffset / 365.2425;
			return (m_lambda + m_alpha * Exp(m_beta * time));
		}

		readonly double m_alpha;
		readonly double m_beta;
		readonly double m_lambda;
	}
}
