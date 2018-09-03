using OneSmallStep.Time;

namespace OneSmallStep.ECS
{
	public abstract class SystemBase
	{
		public abstract void ProcessTick(IEntityLookup entityLookup, NotificationLog eventLog, TimePoint newTime);

		protected abstract ComponentKey GetRequiredComponentsKey(IEntityLookup entityLookup);
	}
}
