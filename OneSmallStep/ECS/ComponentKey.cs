using System;
using System.Numerics;

namespace OneSmallStep.ECS
{
	public struct ComponentKey : IEquatable<ComponentKey>
	{
		public static readonly ComponentKey Empty = new ComponentKey(0);

		public static ComponentKey CreateFromComponentId(int id)
		{
			return new ComponentKey(new BigInteger(1) << id);
		}

		public static ComponentKey operator |(ComponentKey left, ComponentKey right)
		{
			return new ComponentKey(left.m_value | right.m_value);
		}

		public static ComponentKey operator &(ComponentKey left, ComponentKey right)
		{
			return new ComponentKey(left.m_value & right.m_value);
		}

		public static bool operator ==(ComponentKey left, ComponentKey right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ComponentKey left, ComponentKey right)
		{
			return !left.Equals(right);
		}

		public bool Equals(ComponentKey that)
		{
			return m_value.Equals(that.m_value);
		}

		public override bool Equals(object that)
		{
			if (ReferenceEquals(null, that))
				return false;
			return that is ComponentKey key && Equals(key);
		}

		public override int GetHashCode()
		{
			return m_value.GetHashCode();
		}

		private ComponentKey(BigInteger value)
		{
			m_value = value;
		}

		readonly BigInteger m_value;
	}
}
