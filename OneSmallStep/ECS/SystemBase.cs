using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS
{
	public abstract class SystemBase
	{
		public abstract void ProcessTick(IEntityLookup entityLookup, NotificationLog notificationLog, TimePoint newTime);
	}
}
