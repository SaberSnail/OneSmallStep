using OneSmallStep.Utility.Time;
using static System.Math;

namespace OneSmallStep.Utility.Math
{
	public sealed class ExponentialDistribution : ILifetimeDistribution
	{
		public ExponentialDistribution(TimeOffset meanTimeToFailure)
		{
			m_lambda = 1.0 / meanTimeToFailure.TickOffset;
		}

		public double Pdf(TimeOffset t)
		{
			return m_lambda * Exp(-m_lambda * (double) t.TickOffset);
		}

		public double Cdf(TimeOffset t)
		{
			return 1.0 - Exp(-m_lambda * (double) t.TickOffset);
		}

		public double Reliability(TimeOffset t)
		{
			return Exp(-m_lambda * (double) t.TickOffset);
		}

		public double ConditionalReliability(TimeOffset age, TimeOffset t)
		{
			return Exp(-m_lambda * (double) t.TickOffset);
		}

		public double FailureRate(TimeOffset t)
		{
			return m_lambda;
		}

		readonly double m_lambda;
	}
}
