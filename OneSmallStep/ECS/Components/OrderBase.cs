using JetBrains.Annotations;

namespace OneSmallStep.ECS.Components
{
	public abstract class OrderBase
	{
		public OrderId Id { get; }

		public abstract OrderBase Clone();

		protected OrderBase(OrderId orderId)
		{
			Id = orderId;
		}

		protected OrderBase([NotNull] OrderBase that)
		{
			Id = that.Id;
		}
	}
}
