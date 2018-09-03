using OneSmallStep.ECS;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.Utility
{
	public sealed class EventLogItem
	{
		public EventLogItem(TimePoint timeStamp, string message, Entity entity)
		{
			TimeStamp = timeStamp;
			Message = message;
			Entity = entity;
		}

		public TimePoint TimeStamp { get; }

		public string Message { get; }

		public Entity Entity { get; }
	}
}
