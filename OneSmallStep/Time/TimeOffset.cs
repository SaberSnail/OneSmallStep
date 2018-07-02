using System;

namespace OneSmallStep.Time
{
	public struct TimeOffset : IEquatable<TimeOffset>, IComparable<TimeOffset>
	{
		public TimeOffset(long offset)
		{
			TickOffset = offset;
		}

		public long TickOffset { get; }

		public override bool Equals(object that)
		{
			return !ReferenceEquals(that, null) && (that is TimeOffset thatTimeOffset) && Equals(thatTimeOffset);
		}

		public bool Equals(TimeOffset that)
		{
			return TickOffset == that.TickOffset;
		}

		public static bool operator ==(TimeOffset left, TimeOffset right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(TimeOffset left, TimeOffset right)
		{
			return !left.Equals(right);
		}

		public override int GetHashCode()
		{
			return TickOffset.GetHashCode();
		}

		public int CompareTo(TimeOffset that)
		{
			return TickOffset.CompareTo(that.TickOffset);
		}

		public static TimeOffset operator -(TimeOffset left, TimeOffset right)
		{
			return new TimeOffset(left.TickOffset - right.TickOffset);
		}

		public static TimeOffset operator +(TimeOffset left, TimeOffset right)
		{
			return new TimeOffset(left.TickOffset + right.TickOffset);
		}
	}
}