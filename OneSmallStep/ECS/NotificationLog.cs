using System.Collections.Generic;

namespace OneSmallStep.ECS
{
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
