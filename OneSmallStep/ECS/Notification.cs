using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS
{
	public sealed class Notification
	{
		public Notification(string message, TimePoint date, bool shouldStopProcessing, EntityId linkedEntityId)
		{
			Message = message;
			Date = date;
			ShouldStopProcessing = shouldStopProcessing;
			LinkedEntityId = linkedEntityId;
		}

		public string Message { get; }

		public TimePoint Date { get; }

		public bool ShouldStopProcessing { get; }

		public EntityId LinkedEntityId { get; }
	}
}
