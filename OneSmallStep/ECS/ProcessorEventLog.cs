using System.Collections.Generic;

namespace OneSmallStep.ECS
{
	public sealed class ProcessorEvent
	{
		public ProcessorEvent(bool shouldStopProcessing)
		{
			ShouldStopProcessing = shouldStopProcessing;
		}

		public bool ShouldStopProcessing { get; }
	}
	public sealed class ProcessorEventLog
	{
		public ProcessorEventLog()
		{
			m_events = new List<ProcessorEvent>();
		}

		public bool ShouldStopProcessing { get; private set; }

		public IReadOnlyCollection<ProcessorEvent> Events => m_events;

		public void AddEvent(ProcessorEvent e)
		{
			ShouldStopProcessing |= e.ShouldStopProcessing;
			m_events.Add(e);
		}

		public void Reset()
		{
			m_events.Clear();
			ShouldStopProcessing = false;
		}

		readonly List<ProcessorEvent> m_events;
	}
}
