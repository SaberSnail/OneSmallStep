using System;
using System.Diagnostics;

namespace OneSmallStep.Utility.Time
{
	[DebuggerDisplay("{Tick}")]
	public struct TimePoint : IEquatable<TimePoint>, IComparable<TimePoint>
	{
		public TimePoint(long tick)
		{
			Tick = tick;
		}

		public long Tick { get; }

		public TimeOffset GetTimeTo(TimePoint that)
		{
			return new TimeOffset(that.Tick - Tick);
		}

		public override bool Equals(object that)
		{
			return !ReferenceEquals(that, null) && (that is TimePoint thatTimePoint) && Equals(thatTimePoint);
		}

		public bool Equals(TimePoint that)
		{
			return Tick == that.Tick;
		}

		public static bool operator ==(TimePoint left, TimePoint right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(TimePoint left, TimePoint right)
		{
			return !left.Equals(right);
		}

		public override int GetHashCode()
		{
			return Tick.GetHashCode();
		}

		public int CompareTo(TimePoint that)
		{
			return Tick.CompareTo(that.Tick);
		}

		public static bool operator <(TimePoint left, TimePoint right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator <=(TimePoint left, TimePoint right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >(TimePoint left, TimePoint right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator >=(TimePoint left, TimePoint right)
		{
			return left.CompareTo(right) >= 0;
		}

		public static TimePoint operator -(TimePoint timePoint, TimeOffset timeOffset)
		{
			return new TimePoint(timePoint.Tick - timeOffset.TickOffset);
		}

		public static TimePoint operator +(TimePoint timePoint, TimeOffset timeOffset)
		{
			return new TimePoint(timePoint.Tick + timeOffset.TickOffset);
		}
	}
}
