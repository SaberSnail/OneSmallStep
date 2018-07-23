using System.Windows;

namespace OneSmallStep.ECS.Components
{
	public abstract class StarSystemPositionDataBase
	{
		public abstract Point GetAbsolutePosition(OrbitalPositionComponent body);

		public abstract Point GetAbsolutePositionAtTime(OrbitalPositionComponent body, double ticks);

		public abstract Point? GetInterceptPoint(OrbitalPositionComponent body, Point interceptorPosition, double interceptorMaxSpeed);

		public abstract void EnsureStartValidity(OrbitalPositionComponent body);

		public abstract void EnsureEndValidity(OrbitalPositionComponent body);

		public abstract void MoveOneTick(OrbitalPositionComponent body);

		public virtual void TrySetTarget(Entity target) { }

		public virtual Point? TryGetTargetPoint() => null;
	}
}
