using System.Windows;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS.Components
{
	public abstract class MovementOrderBase
	{
		public abstract bool PrepareIntercept(IEntityLookup entityLookup, Point currentAbsolutePosition, double maxSpeedPerTick, TimePoint currentTime);
		public abstract bool TryMarkAsResolved(IEntityLookup entityLookup, Point currentAbsolutePosition);
		public abstract Point MoveOneTick(Point currentAbsolutePosition);
		public abstract MovementOrderBase Clone();
	}
}
