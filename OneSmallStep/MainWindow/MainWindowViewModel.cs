using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GoldenAnvil.Utility.Logging;
using OneSmallStep.ECS;
using OneSmallStep.ECS.Components;
using OneSmallStep.ECS.Systems;
using OneSmallStep.EntityViewModels;
using OneSmallStep.SystemMap;
using OneSmallStep.Time;
using OneSmallStep.Utility;

namespace OneSmallStep.MainWindow
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel(GameServices gameServices)
		{
			m_gameServices = gameServices;
			m_systemMap = new SystemMapViewModel();
			m_planets = new List<PlanetViewModel>();
			m_ships = new List<ShipViewModel>();
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

		public ShipViewModel Ship
		{
			get
			{
				VerifyAccess();
				return m_ship;
			}
			private set
			{
				SetPropertyField(nameof(Ship), value, ref m_ship);
			}
		}

		public SystemMapViewModel SystemMap
		{
			get
			{
				VerifyAccess();
				return m_systemMap;
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
				new StarSystemMovementSystem(m_gameData),
			};

			InitializeEntities();

			UpdateCurrentDate();

			IsGameStarted = true;

			m_stopwatchMilestone = m_gameData.Calendar.CreateTimePoint(2100, 1, 1);
			m_stopwatch = Stopwatch.StartNew();

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

			foreach (var planet in m_planets)
				planet.UpdateFromEntity();
			foreach (var ship in m_ships)
				ship.UpdateFromEntity();

			foreach (var ship in m_ships)
			{
				if (ship.TargetPosition == null)
					ship.Entity.GetComponent<OrbitalPositionComponent>().TrySetTarget(m_planets[m_gameServices.RandomNumberGenerator.Next(0, m_planets.Count - 1)].Entity);
			}

			UpdateCurrentDate();

			if (m_gameData.CurrentDate == m_stopwatchMilestone)
				Log.Info($"Got to {m_gameData.Calendar.FormatTime(m_stopwatchMilestone, TimeFormat.Short)} in {m_stopwatch.Elapsed.TotalSeconds}");

			if (ShouldRunAtFullSpeed)
				Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action) ProcessTick);
		}

		private void UpdateCurrentDate()
		{
			var date = m_gameData.Calendar.FormatTime(m_gameData.CurrentDate, TimeFormat.Long);
			SetPropertyField(nameof(CurrentDate), date, ref m_currentDate);

			m_systemMap.Update(date, m_planets.Cast<ISystemBodyRenderer>().Concat(m_ships).ToList());
		}

		private void InitializeEntities()
		{
			var sun = m_gameData.EntityManager.CreatePlanet(1.9885E30);
			m_planets.Add(new PlanetViewModel(sun));

			var mercury = m_gameData.EntityManager.CreatePlanet(sun, 3.3011E23, 87.9691, 174.796, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(mercury));

			var venus = m_gameData.EntityManager.CreatePlanet(sun, 4.8675E24, 224.701, 50.115, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(venus));

			var earth = m_gameData.EntityManager.CreatePlanet(sun, 5.97237E24, 365.256363004, 358.617, m_gameData.Calendar);
			var population = new PopulationComponent { Population = 1000000000 };
			earth.AddComponent(population);
			Planet = new PlanetViewModel(earth);
			m_planets.Add(Planet);
			var moon = m_gameData.EntityManager.CreatePlanet(earth, 7.342E22, 27.321661, 134.96292, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(moon));

			var mars = m_gameData.EntityManager.CreatePlanet(sun, 6.4171E23, 686.971, 320.45776, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(mars));
			var phobos = m_gameData.EntityManager.CreatePlanet(mars, 4.0659E16, 0.31891023, 0, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(phobos));
			var deimos = m_gameData.EntityManager.CreatePlanet(mars, 1.4762E15, 1.263, 0, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(deimos));

			foreach (var planet in m_planets)
				planet.UpdateFromEntity();

			var ship1 = m_gameData.EntityManager.CreateShip(new Point(1E12, 1E12));
			Ship = new ShipViewModel(ship1);
			m_ships.Add(Ship);

			foreach (var ship in m_ships)
				ship.UpdateFromEntity();
		}

		static readonly ILogSource Log = LogManager.CreateLogSource(nameof(MainWindowViewModel));

		readonly GameServices m_gameServices;
		readonly SystemMapViewModel m_systemMap;
		readonly List<PlanetViewModel> m_planets;
		readonly List<ShipViewModel> m_ships;

		GameData m_gameData;
		bool m_isGameStarted;
		string m_currentDate;
		PlanetViewModel m_planet;
		IReadOnlyList<SystemBase> m_systems;
		bool m_shouldRunAtFullSpeed;
		Stopwatch m_stopwatch;
		TimePoint m_stopwatchMilestone;
		ShipViewModel m_ship;
	}
}
