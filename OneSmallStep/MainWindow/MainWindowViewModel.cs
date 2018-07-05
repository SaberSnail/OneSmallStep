using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;
using GoldenAnvil.Utility.Logging;
using OneSmallStep.ECS;
using OneSmallStep.ECS.Systems;
using OneSmallStep.EntityViewModels;
using OneSmallStep.Time;
using OneSmallStep.Utility;

namespace OneSmallStep.MainWindow
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel(GameServices gameServices)
		{
			m_gameServices = gameServices;
		}

		public bool IsGameStarted
		{
			get
			{
				VerifyAccess();
				return m_isGameStarted;
			}
			private set
			{
				if (SetPropertyField(nameof(IsGameStarted), value, ref m_isGameStarted))
					Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested, DispatcherPriority.Normal);
			}
		}

		public bool ShouldRunAtFullSpeed
		{
			get
			{
				VerifyAccess();
				return m_shouldRunAtFullSpeed;
			}
			set
			{
				SetPropertyField(nameof(ShouldRunAtFullSpeed), value, ref m_shouldRunAtFullSpeed);
			}
		}

		public string CurrentDate
		{
			get
			{
				VerifyAccess();
				return m_currentDate;
			}
		}

		public PlanetViewModel Planet
		{
			get
			{
				VerifyAccess();
				return m_planet;
			}
			private set
			{
				SetPropertyField(nameof(Planet), value, ref m_planet);
			}
		}

		public void StartGame()
		{
			if (IsGameStarted)
				return;

			m_gameData = new GameData(StandardCalendar.Create(2050, 1, 1, Constants.TicksPerDay));
			m_systems = new[]
			{
				new PopulationGrowthSystem(m_gameData, m_gameServices.RandomNumberGenerator),
			};

			m_planetEntity = m_gameData.EntityManager.CreatePlanet(m_gameServices.RandomNumberGenerator);
			Planet = new PlanetViewModel(m_planetEntity);
			Planet.UpdateFromEntity();

			UpdateCurrentDate();

			IsGameStarted = true;

			if (ShouldRunAtFullSpeed)
				Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action) ProcessTick);
		}

		public void ProcessTick()
		{
			if (!IsGameStarted)
				return;

			m_gameData.CurrentDate = m_gameData.CurrentDate + Constants.Tick;

			foreach (var system in m_systems)
				system.Process();

			UpdateCurrentDate();
			m_planet.UpdateFromEntity();

			if (ShouldRunAtFullSpeed)
				Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action) ProcessTick);
		}

		private void UpdateCurrentDate()
		{
			var date = m_gameData.Calendar.FormatTime(m_gameData.CurrentDate, TimeFormat.Long);
			SetPropertyField(nameof(CurrentDate), date, ref m_currentDate);
		}

		static readonly ILogSource Log = LogManager.CreateLogSource(nameof(MainWindowViewModel));

		readonly GameServices m_gameServices;

		GameData m_gameData;
		Entity m_planetEntity;
		bool m_isGameStarted;
		string m_currentDate;
		PlanetViewModel m_planet;
		IReadOnlyList<SystemBase> m_systems;
		bool m_shouldRunAtFullSpeed;
	}
}
