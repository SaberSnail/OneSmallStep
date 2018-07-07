using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;
using GoldenAnvil.Utility.Logging;
using OneSmallStep.ECS;
using OneSmallStep.ECS.Components;
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
			m_systems = new SystemBase[]
			{
				new PopulationGrowthSystem(m_gameData, m_gameServices.RandomNumberGenerator),
				new OrbitalDynamicsSystem(m_gameData), 
			};

			InitializeEntities();

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

		private void InitializeEntities()
		{
			var sun = m_gameData.EntityManager.CreatePlanet(1.9885E30);

			var earth = m_gameData.EntityManager.CreatePlanet(sun, 5.97237E24, 365.256363004, 358.617, m_gameData.Calendar);
			var population = new PopulationComponent { Population = 1000000000 };
			earth.AddComponent(population);

			var moon = m_gameData.EntityManager.CreatePlanet(earth, 7.342E22, 27.321661, 134.96292, m_gameData.Calendar);

			Planet = new PlanetViewModel(earth);
			Planet.UpdateFromEntity();
		}

		static readonly ILogSource Log = LogManager.CreateLogSource(nameof(MainWindowViewModel));

		readonly GameServices m_gameServices;

		GameData m_gameData;
		bool m_isGameStarted;
		string m_currentDate;
		PlanetViewModel m_planet;
		IReadOnlyList<SystemBase> m_systems;
		bool m_shouldRunAtFullSpeed;
	}
}
