using System.Collections.Generic;
using OneSmallStep.Time;

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
	public sealed class NotificationLog
	{
		public NotificationLog()
		{
			m_events = new List<Notification>();
		}

		public bool ShouldStopProcessing { get; private set; }

		public IReadOnlyCollection<Notification> Events => m_events;

		public void AddEvent(Notification e)
		{
			ShouldStopProcessing |= e.ShouldStopProcessing;
			m_events.Add(e);
		}

		public void Reset()
		{
			m_events.Clear();
			ShouldStopProcessing = false;
		}

		readonly List<Notification> m_events;
	}
}
