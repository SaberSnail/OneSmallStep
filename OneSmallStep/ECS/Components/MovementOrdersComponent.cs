using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace OneSmallStep.ECS.Components
{
	public sealed class MovementOrdersComponent : ComponentBase
	{
		public MovementOrdersComponent()
		{
			m_orders = new List<MovementOrderBase>();
		}

		public bool HasActiveOrder()
		{
			return m_orders.Count != 0;
		}

		public MovementOrderBase GetActiveOrder()
		{
			return m_orders.FirstOrDefault()?.Clone();
		}

		public void AddOrderToBack(MovementOrderBase order)
		{
			using (ScopedPropertyChange())
				m_orders.Add(order.Clone());
		}

		public void PrepareIntercept(IEntityLookup entityLookup, Point currentAbsolutePosition, double speedPerTick)
		{
			if (m_orders[0].PrepareIntercept(entityLookup, currentAbsolutePosition, speedPerTick))
				SetChanged();
		}

		public void ResolveOrderIfNeeded(IEntityLookup entityLookup, Point currentAbsolutePosition)
		{
			if (m_orders[0].TryMarkAsResolved(entityLookup, currentAbsolutePosition))
			{
				using (ScopedPropertyChange())
					m_orders.RemoveAt(0);
			}
		}

		public Point MoveOneTick(Point currentAbsolutePosition)
		{
			return m_orders[0].MoveOneTick(currentAbsolutePosition);
		}

		public override ComponentBase Clone()
		{
			return new MovementOrdersComponent(this);
		}

		private MovementOrdersComponent(MovementOrdersComponent that)
			: base(that)
		{
			m_orders = that.m_orders.Select(x => x.Clone()).ToList();
		}

		readonly List<MovementOrderBase> m_orders;
	}
}
