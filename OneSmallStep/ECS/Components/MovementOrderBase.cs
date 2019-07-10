using System.Windows;
using JetBrains.Annotations;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS.Components
{
	public abstract class MovementOrderBase : OrderBase
	{
		public abstract MovementOrderBase PrepareIntercept(IEntityLookup entityLookup, Point currentAbsolutePosition, double maxSpeedPerTick, TimePoint currentTime);

		public abstract bool TryMarkAsResolved(IEntityLookup entityLookup, Point currentAbsolutePosition, out MovementOrderBase newOrder);

		public abstract Point MoveOneTick(Point currentAbsolutePosition);

		protected MovementOrderBase(OrderId orderId)
			: base(orderId)
		{
		}

		protected MovementOrderBase([NotNull] OrderBase that)
			: base(that)
		{
		}
	}
}
