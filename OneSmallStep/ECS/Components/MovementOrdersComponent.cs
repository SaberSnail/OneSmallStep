using System.Collections.Generic;
using System.Windows;

namespace OneSmallStep.ECS.Components
{
	public sealed class MovementOrdersComponent : ComponentBase
	{
		public MovementOrdersComponent()
		{
			Orders = new List<MovementOrderBase>();
		}

		public List<MovementOrderBase> Orders { get; }

		public void PrepareIntercept(Point currentAbsolutePosition, double speedPerTick)
		{
			Orders[0].PrepareIntercept(currentAbsolutePosition, speedPerTick);
		}

		public void ResolveOrderIfNeeded(Point currentAbsolutePosition)
		{
			if (Orders[0].TryMarkAsResolved(currentAbsolutePosition))
				Orders.RemoveAt(0);
		}

		public Point MoveOneTick(Point currentAbsolutePosition)
		{
			return Orders[0].MoveOneTick(currentAbsolutePosition);
		}
	}
}
