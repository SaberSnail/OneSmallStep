using System;
using System.Diagnostics;

namespace OneSmallStep.ECS.Components
{
	[DebuggerDisplay("{m_id}")]
	public struct OrderId : IEquatable<OrderId>
	{
		public OrderId(uint id) => m_id = id;

		public override int GetHashCode() => m_id.GetHashCode();

		public override bool Equals(object that) => that is OrderId id && Equals(id);

		public bool Equals(OrderId that) => m_id == that.m_id;

		public static bool operator ==(OrderId left, OrderId right) => left.Equals(right);

		public static bool operator !=(OrderId left, OrderId right) => !left.Equals(right);

		public override string ToString() => m_id.ToString();

		readonly uint m_id;
	}
}
