using System.Collections.Generic;
using System.Linq;

namespace OneSmallStep.ECS.Components
{
	public sealed class OrdersComponent : ComponentBase
	{
		public OrdersComponent()
		{
			m_orders = new List<OrderBase>();
		}

		public bool HasActiveOrder<T>() => m_orders.OfType<T>().Any();

		public T GetActiveOrder<T>() => m_orders.OfType<T>().FirstOrDefault();

		public override ComponentBase Clone() => new OrdersComponent(this);

		public void AddOrderToBack(OrderBase order)
		{
			using (ScopedPropertyChange())
				m_orders.Add(order.Clone());
		}

		public void RemoveOrder(OrderBase order)
		{
			using (ScopedPropertyChange())
				m_orders.Remove(order);
		}

		public void UpdateOrder(OrderBase order)
		{
			using (ScopedPropertyChange())
			{
				int i = m_orders.FindIndex(x => x.Id == order.Id);
				m_orders[i] = order;
			}
		}

		private OrdersComponent(OrdersComponent that)
			: base(that)
		{
			m_orders = that.m_orders.Select(x => x.Clone()).ToList();
		}

		readonly List<OrderBase> m_orders;
	}
}
