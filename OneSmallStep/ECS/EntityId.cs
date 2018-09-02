﻿using System;

namespace OneSmallStep.ECS
{
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

		readonly int m_id;
	}
}
