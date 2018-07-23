using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GoldenAnvil.Utility;
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

			ShouldRunAtFullSpeed = true;
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
				var shipEntity = ship.Entity.GetComponent<OrbitalPositionComponent>();
				shipEntity.TrySetTarget(m_planets[m_gameServices.RandomNumberGenerator.Next(0, m_planets.Count - 1)].Entity);
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
			var sun = m_gameData.EntityManager.CreatePlanet(1.9885E30, 6.957E8);
			m_planets.Add(new PlanetViewModel(sun));

			var mercury = m_gameData.EntityManager.CreatePlanet(sun, 3.3011E23, 2.4397E6, 87.9691, 174.796, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(mercury));

			var venus = m_gameData.EntityManager.CreatePlanet(sun, 4.8675E24, 6.0518E6, 224.701, 50.115, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(venus));

			var earth = m_gameData.EntityManager.CreatePlanet(sun, 5.97237E24, 6.371E6, 365.256363004, 358.617, true, m_gameData.Calendar);
			var population = new PopulationComponent { Population = 1000000000 };
			earth.AddComponent(population);
			Planet = new PlanetViewModel(earth);
			m_planets.Add(Planet);
			var moon = m_gameData.EntityManager.CreatePlanet(earth, 7.342E22, 1.7371E6, 27.321661, 134.96292, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(moon));

			var mars = m_gameData.EntityManager.CreatePlanet(sun, 6.4171E23, 3.3895E6, 686.971, 320.45776, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(mars));
			var phobos = m_gameData.EntityManager.CreatePlanet(mars, 4.0659E16, 11266.7, 0.31891023, 92.474, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(phobos));
			var deimos = m_gameData.EntityManager.CreatePlanet(mars, 1.4762E15, 6200, 1.263, 296.23, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(deimos));

			var jupiter = m_gameData.EntityManager.CreatePlanet(sun, 1.8982E27, 6.9911E7, 4332.59, 20.02, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(jupiter));
			var metis = m_gameData.EntityManager.CreatePlanet(jupiter, 3.6E16, 21500, 0.294780, 276.047, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(metis));
			var adrastea = m_gameData.EntityManager.CreatePlanet(jupiter, 2E15, 8200, 0.29826, 328.047, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(adrastea));
			var amalthea = m_gameData.EntityManager.CreatePlanet(jupiter, 2.08E18, 83500, 0.49817943, 185.194, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(amalthea));
			var thebe = m_gameData.EntityManager.CreatePlanet(jupiter, 4.3E17, 49300, 0.674536, 135.956, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(thebe));
			var io = m_gameData.EntityManager.CreatePlanet(jupiter, 8.931938E22, 1.8216E6, 1.769137786, 342.021, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(io));
			var europa = m_gameData.EntityManager.CreatePlanet(jupiter, 4.799844E22, 1.5608E6, 3.551181, 171.016, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(europa));
			var ganymede = m_gameData.EntityManager.CreatePlanet(jupiter, 1.4819E23, 2.6341E6, 7.15455296, 317.54, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(ganymede));
			var callisto = m_gameData.EntityManager.CreatePlanet(jupiter, 1.075938E23, 2.4103E6, 16.6890184, 181.408, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(callisto));

			var saturn = m_gameData.EntityManager.CreatePlanet(sun, 5.6834E26, 5.8232E7, 10759.22, 317.02, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(saturn));
			var mimas = m_gameData.EntityManager.CreatePlanet(saturn, 3.7493E19, 198200, 0.942, 255.312, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(mimas));
			var enceladus = m_gameData.EntityManager.CreatePlanet(saturn, 1.08022E20, 252100, 1.370218, 197.047, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(enceladus));
			var tethys = m_gameData.EntityManager.CreatePlanet(saturn, 6.17449E20, 5.0311E6, 1.887802, 189.003, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(tethys));
			var dione = m_gameData.EntityManager.CreatePlanet(saturn, 1.095452E21, 561400, 2.736915, 65.99, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(dione));
			var rhea = m_gameData.EntityManager.CreatePlanet(saturn, 2.306218E21, 763800, 4.518212, 311.551, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(rhea));
			var titan = m_gameData.EntityManager.CreatePlanet(saturn, 1.3452E23, 2575.5, 15.945, 15.154, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(titan));
			var hyperion = m_gameData.EntityManager.CreatePlanet(saturn, 5.6199E18, 180.1, 21.276, 295.906, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(hyperion));
			var iapetus = m_gameData.EntityManager.CreatePlanet(saturn, 1.805635E21, 367.2, 79.3215, 356.029, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(iapetus));
			var phoebe = m_gameData.EntityManager.CreatePlanet(saturn, 8.292E18, 106.5, 550.564636, 287.593, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(phoebe));

			var uranus = m_gameData.EntityManager.CreatePlanet(sun, 8.6810E25, 2.5362E7, 30688.5, 142.2386, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(uranus));
			var miranda = m_gameData.EntityManager.CreatePlanet(uranus, 6.59E19, 235800, 1.413479, 311.33, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(miranda));
			var ariel = m_gameData.EntityManager.CreatePlanet(uranus, 1.353E21, 578900, 2.52, 39.481, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(ariel));
			var umbriel = m_gameData.EntityManager.CreatePlanet(uranus, 1.172E21, 584700, 4.144, 12.469, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(umbriel));
			var titania = m_gameData.EntityManager.CreatePlanet(uranus, 3.527E21, 788400, 8.706234, 24.614, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(titania));
			var oberon = m_gameData.EntityManager.CreatePlanet(uranus, 3.014E21, 761400, 13.463234, 283.088, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(oberon));

			var neptune = m_gameData.EntityManager.CreatePlanet(sun, 1.0243E26, 2.4622E7, 60182, 256.228, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(neptune));
			var triton = m_gameData.EntityManager.CreatePlanet(neptune, 2.14E22, 1.3534E6, 5.876854, 264.775, false, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(triton));
			var nereid = m_gameData.EntityManager.CreatePlanet(neptune, 3E19, 170000, 360.1362, 359.341, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(nereid));
			var proteus = m_gameData.EntityManager.CreatePlanet(neptune, 4.4E19, 210000, 1.12231477, 117.050, true, m_gameData.Calendar);
			m_planets.Add(new PlanetViewModel(proteus));

			foreach (var planet in m_planets)
				planet.UpdateFromEntity();

			var ship1 = m_gameData.EntityManager.CreateShip(new Point(m_gameServices.RandomNumberGenerator.NextDouble(-1E12, 1E12), m_gameServices.RandomNumberGenerator.NextDouble(-1E12, 1E12)));
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
