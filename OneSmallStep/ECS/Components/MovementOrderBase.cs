using System.Windows;

namespace OneSmallStep.ECS.Components
{
	public abstract class MovementOrderBase
	{
		public abstract void PrepareIntercept(Point currentAbsolutePosition, double maxSpeedPerTick);
		public abstract bool TryMarkAsResolved(Point currentAbsolutePosition);
		public abstract Point MoveOneTick(Point currentAbsolutePosition);
	}
}
