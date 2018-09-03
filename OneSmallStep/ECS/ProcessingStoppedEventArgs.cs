using System;
using System.Collections.Generic;

namespace OneSmallStep.ECS
{
	public class ProcessingStoppedEventArgs : EventArgs
	{
		public ProcessingStoppedEventArgs(IReadOnlyCollection<Notification> notifications)
		{
			Notifications = notifications;
		}

		public IReadOnlyCollection<Notification> Notifications { get; }
	}
}
