using OneSmallStep.Time;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.Utility
{
	public static class Constants
	{
		public const double Ln2 = 0.69314718055994530941723212145818;

		public const double GravitationalConstant = 6.67408E-11;

		public const double TicksPerDay = 1;
		public const double SecondsPerTick = 24.0 * 60.0 * 60.0 / TicksPerDay;
		public static readonly TimeOffset Tick = new TimeOffset(1);
	}
}
