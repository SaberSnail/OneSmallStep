using System;
using System.Diagnostics;

namespace OneSmallStep.ECS
{
	[DebuggerDisplay("{m_id}")]
	public struct EntityId : IEquatable<EntityId>
	{
		public EntityId(int id)
		{
			m_id = id;
		}

		public override int GetHashCode()
		{
			return m_id.GetHashCode();
		}

		public override bool Equals(object that)
		{
			return that is EntityId id && Equals(id);
		}

		public bool Equals(EntityId that)
		{
			return m_id == that.m_id;
		}

		public static bool operator ==(EntityId left, EntityId right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(EntityId left, EntityId right)
		{
			return !left.Equals(right);
		}

		public override string ToString()
		{
			return m_id.ToString();
		}

		readonly int m_id;
	}
}
