using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Logging;
using OneSmallStep.Utility;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS
{
	public sealed class GameProcessor : IDisposable
	{
		public GameProcessor()
		{
			m_systems = new List<SystemBase>();

			m_threadContinueEvent = new AutoResetEvent(false);
			m_threadStopEvent = new AutoResetEvent(false);
			m_threadStoppedEvent = new AutoResetEvent(false);
			m_threadWaitHandles = new WaitHandle[] { m_threadStopEvent, m_threadContinueEvent };

			m_dispatcher = Dispatcher.CurrentDispatcher;

			m_workThread = new Thread(DoProcessing);
		}

		public event EventHandler<ProcessingStoppedEventArgs> ProcessingStopped;

		public void RegisterSystem(SystemBase system)
		{
			m_systems.Add(system);
		}

		public void RegisterGameData(GameData gameData)
		{
			m_gameData = gameData;
			m_currentDate = gameData.CurrentDate;
			m_entityManager = gameData.EntityManager;
			m_workThread.Start();
		}

		public void StartRunning()
		{
			Log.Info("Continuing processing");
			m_threadContinueEvent.Set();
		}

		public void Dispose()
		{
			if (m_workThread.IsAlive)
			{
				m_threadStopEvent.Set();
				m_threadStoppedEvent.WaitOne();
			}
		}

		private void DoProcessing()
		{
			var entityLookup = m_entityManager.ProcessingEntityLookup;
			var notificationLog = new NotificationLog();
			m_currentDate = m_gameData.CurrentDate;

			try
			{
				while (true)
				{
					if (WaitHandle.WaitAny(m_threadWaitHandles) == c_stopEventIndex)
						break;

					m_gameData.EntityManager.UpdateProcessingEntitiesFromDisplay();

					m_currentDate = m_currentDate + Constants.Tick;

					foreach (var system in m_systems)
						system.ProcessTick(entityLookup, notificationLog, m_currentDate);

					//var shouldStop = m_currentDate >= m_gameData.Calendar.CreateTimePoint(2100, 1, 1);
					if (notificationLog.ShouldStopProcessing)
					{
						m_threadContinueEvent.Reset();
						Log.Info("Pausing processing");

						var notifications = notificationLog.Events.ToList();
						notificationLog.Reset();
						m_dispatcher.Invoke(() =>
						{
							m_gameData.CurrentDate = m_currentDate;
							m_gameData.EntityManager.SwapDisplayWithProcessing();
							ProcessingStopped.Raise(this, new ProcessingStoppedEventArgs(notifications));
						});
					}
					else
					{
						m_threadContinueEvent.Set();
					}
				}
			}
			finally
			{
				m_threadStoppedEvent.Set();
			}
		}

		static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(GameProcessor));

		const int c_stopEventIndex = 0;

		readonly WaitHandle[] m_threadWaitHandles;
		readonly AutoResetEvent m_threadContinueEvent;
		readonly AutoResetEvent m_threadStopEvent;
		readonly AutoResetEvent m_threadStoppedEvent;
		readonly List<SystemBase> m_systems;
		readonly Dispatcher m_dispatcher;
		readonly Thread m_workThread;

		GameData m_gameData;
		TimePoint m_currentDate;
		EntityManager m_entityManager;
	}
}
