using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Logging;
using OneSmallStep.ECS;
using OneSmallStep.ECS.Components;
using OneSmallStep.UI.EntityViewModels;
using OneSmallStep.UI.SystemMap;
using OneSmallStep.UI.Utility;
using OneSmallStep.Utility;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.UI.MainWindow
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel(GameServices gameServices)
		{
			m_gameServices = gameServices;
			m_systemMap = new SystemMapViewModel();
			m_planets = new List<PlanetViewModel>();
			m_ships = new List<ShipViewModel>();
			AppModel = ((App) Application.Current).AppModel;
		}

		public AppModel AppModel { get; }

		public bool IsGameStarted
		{
			get
			{
				VerifyAccess();
				return m_isGameStarted;
			}
			private set
			{
				if (SetPropertyField(value, ref m_isGameStarted))
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
				SetPropertyField(value, ref m_shouldRunAtFullSpeed);
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
				SetPropertyField(value, ref m_planet);
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
				SetPropertyField(value, ref m_ship);
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

		public string Status
		{
			get
			{
				VerifyAccess();
				return m_tokenStringTest;
			}
			private set
			{
				if (SetPropertyField(value, ref m_tokenStringTest))
					CommandManager.InvalidateRequerySuggested();
			}
		}

		public void StartGame()
		{
			if (IsGameStarted)
				return;

			AppModel.GameData = new GameData(StandardCalendar.Create(2050, 1, 1, Constants.TicksPerDay));
			m_gameServices.Processor.RegisterGameData(AppModel.GameData);
			m_gameServices.Processor.ProcessingStopped += OnProcessingStopped;

			InitializeEntities();

			UpdateCurrentDate();
			UpdateShipOrders();

			IsGameStarted = true;
		}

		public void StartProcessing()
		{
			//if (AppModel.CurrentTheme == new Uri(@"/UI/Themes/Default/Default.xaml", UriKind.Relative))
			//	AppModel.CurrentTheme = new Uri(@"/UI/Themes/Test/Test.xaml", UriKind.Relative);
			//else
			//	AppModel.CurrentTheme = new Uri(@"/UI/Themes/Default/Default.xaml", UriKind.Relative);

			m_gameServices.Processor.StartRunning();
		}

		public void GoToEntity(EntityId entityId)
		{
			var entityLookup = AppModel.GameData.EntityManager.DisplayEntityLookup;
			m_systemMap.GoToEntity(entityId, entityLookup);
		}

		private void UpdateShipOrders()
		{
			var entityLookup = AppModel.GameData.EntityManager.DisplayEntityLookup;

			foreach (var ship in m_ships.Select(x => entityLookup.GetEntity(x.EntityId)))
			{
				var orders = ship.GetRequiredComponent<MovementOrdersComponent>();
				if (!orders.HasActiveOrder())
				{
					var newTargetId = m_planets[m_gameServices.RandomNumberGenerator.Next(0, m_planets.Count - 1)].EntityId;
					var newTarget = entityLookup.GetEntity(newTargetId);
					var speed = ship.GetRequiredComponent<OrbitalUnitDesignComponent>().MaxSpeedPerTick;
					orders.AddOrderToBack(new MoveToOrbitalBodyOrder(newTarget.Id, speed));
					//newTargetId = m_planets[m_gameServices.RandomNumberGenerator.Next(0, m_planets.Count - 1)].EntityId;
					//newTarget = entityLookup.GetEntity(newTargetId);
					//orders.AddOrderToBack(new MoveToOrbitalBodyOrder(newTarget.Id, speed));

					Status = $"{{{ship.Id}.InformationComponent.Name}} is headed to {{{newTargetId}.InformationComponent.Name}}.";
				}
			}
		}

		private void OnProcessingStopped(object sender, ProcessingStoppedEventArgs e)
		{
			UpdateShipOrders();

			m_refreshOperation?.Abort();
			m_refreshOperation = Dispatcher.InvokeAsync(() =>
			{
				var entityLookup = AppModel.GameData.EntityManager.DisplayEntityLookup;

				foreach (var planet in m_planets)
					planet.UpdateFromEntity(entityLookup);
				foreach (var ship in m_ships)
					ship.UpdateFromEntity(entityLookup);

				UpdateCurrentDate();

				if (ShouldRunAtFullSpeed)
					m_gameServices.Processor.StartRunning();
			}, DispatcherPriority.Background);

			foreach (var notification in e.Notifications)
				Log.Info($"{AppModel.GameData.Calendar.FormatTime(notification.Date, TimeFormat.Short)} - {TokenStringUtility.GetString(notification.Message, AppModel.GameData.EntityManager)}");
		}

		private void UpdateCurrentDate()
		{
			var date = AppModel.GameData.Calendar.FormatTime(AppModel.GameData.CurrentDate, TimeFormat.Long);
			SetPropertyField(nameof(CurrentDate), date, ref m_currentDate);

			m_systemMap.Update(date, m_planets.Cast<ISystemBodyRenderer>().Concat(m_ships).ToList());
		}

		private void InitializeEntities()
		{
			var gameData = AppModel.GameData;
			var entityLookup = gameData.EntityManager.DisplayEntityLookup;

			var sol = EntityUtility.CreatePlanet(entityLookup, "Sol", 1.9885E30, 6.957E8);
			m_planets.Add(new PlanetViewModel(sol));

			var mercury = EntityUtility.CreatePlanet(entityLookup, "Mercury", sol, 3.3011E23, 2.4397E6, 87.9691, 174.796, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(mercury));

			var venus = EntityUtility.CreatePlanet(entityLookup, "Venus", sol, 4.8675E24, 6.0518E6, 224.701, 50.115, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(venus));

			var earth = EntityUtility.CreatePlanet(entityLookup, "Earth", sol, 5.97237E24, 6.371E6, 365.256363004, 358.617, true, gameData.Calendar);
			var population = new PopulationComponent { Population = 1000000000 };
			earth.AddComponent(population);
			Planet = new PlanetViewModel(earth);
			m_planets.Add(Planet);
			var luna = EntityUtility.CreatePlanet(entityLookup, "Luna", earth, 7.342E22, 1.7371E6, 27.321661, 134.96292, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(luna));

			var mars = EntityUtility.CreatePlanet(entityLookup, "Mars", sol, 6.4171E23, 3.3895E6, 686.971, 320.45776, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(mars));
			var phobos = EntityUtility.CreatePlanet(entityLookup, "Phobos", mars, 4.0659E16, 11266.7, 0.31891023, 92.474, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(phobos));
			var deimos = EntityUtility.CreatePlanet(entityLookup, "Deimos", mars, 1.4762E15, 6200, 1.263, 296.23, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(deimos));

			var jupiter = EntityUtility.CreatePlanet(entityLookup, "Jupiter", sol, 1.8982E27, 6.9911E7, 4332.59, 20.02, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(jupiter));
			var metis = EntityUtility.CreatePlanet(entityLookup, "Metis", jupiter, 3.6E16, 21500, 0.294780, 276.047, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(metis));
			var adrastea = EntityUtility.CreatePlanet(entityLookup, "Adrastea", jupiter, 2E15, 8200, 0.29826, 328.047, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(adrastea));
			var amalthea = EntityUtility.CreatePlanet(entityLookup, "Amalthea", jupiter, 2.08E18, 83500, 0.49817943, 185.194, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(amalthea));
			var thebe = EntityUtility.CreatePlanet(entityLookup, "Thebe", jupiter, 4.3E17, 49300, 0.674536, 135.956, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(thebe));
			var io = EntityUtility.CreatePlanet(entityLookup, "Io", jupiter, 8.931938E22, 1.8216E6, 1.769137786, 342.021, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(io));
			var europa = EntityUtility.CreatePlanet(entityLookup, "Europa", jupiter, 4.799844E22, 1.5608E6, 3.551181, 171.016, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(europa));
			var ganymede = EntityUtility.CreatePlanet(entityLookup, "Ganymede", jupiter, 1.4819E23, 2.6341E6, 7.15455296, 317.54, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(ganymede));
			var callisto = EntityUtility.CreatePlanet(entityLookup, "Callisto", jupiter, 1.075938E23, 2.4103E6, 16.6890184, 181.408, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(callisto));

			var saturn = EntityUtility.CreatePlanet(entityLookup, "Saturn", sol, 5.6834E26, 5.8232E7, 10759.22, 317.02, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(saturn));
			var mimas = EntityUtility.CreatePlanet(entityLookup, "Mimas", saturn, 3.7493E19, 198200, 0.942, 255.312, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(mimas));
			var enceladus = EntityUtility.CreatePlanet(entityLookup, "Enceladus", saturn, 1.08022E20, 252100, 1.370218, 197.047, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(enceladus));
			var tethys = EntityUtility.CreatePlanet(entityLookup, "Tethys", saturn, 6.17449E20, 5.0311E6, 1.887802, 189.003, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(tethys));
			var dione = EntityUtility.CreatePlanet(entityLookup, "Dione", saturn, 1.095452E21, 561400, 2.736915, 65.99, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(dione));
			var rhea = EntityUtility.CreatePlanet(entityLookup, "Rhea", saturn, 2.306218E21, 763800, 4.518212, 311.551, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(rhea));
			var titan = EntityUtility.CreatePlanet(entityLookup, "Titan", saturn, 1.3452E23, 2575.5, 15.945, 15.154, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(titan));
			var hyperion = EntityUtility.CreatePlanet(entityLookup, "Hyperion", saturn, 5.6199E18, 180.1, 21.276, 295.906, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(hyperion));
			var iapetus = EntityUtility.CreatePlanet(entityLookup, "Iapetus", saturn, 1.805635E21, 367.2, 79.3215, 356.029, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(iapetus));
			var phoebe = EntityUtility.CreatePlanet(entityLookup, "Phoebe", saturn, 8.292E18, 106.5, 550.564636, 287.593, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(phoebe));

			var uranus = EntityUtility.CreatePlanet(entityLookup, "Uranus", sol, 8.6810E25, 2.5362E7, 30688.5, 142.2386, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(uranus));
			var miranda = EntityUtility.CreatePlanet(entityLookup, "Miranda", uranus, 6.59E19, 235800, 1.413479, 311.33, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(miranda));
			var ariel = EntityUtility.CreatePlanet(entityLookup, "Ariel", uranus, 1.353E21, 578900, 2.52, 39.481, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(ariel));
			var umbriel = EntityUtility.CreatePlanet(entityLookup, "Umbriel", uranus, 1.172E21, 584700, 4.144, 12.469, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(umbriel));
			var titania = EntityUtility.CreatePlanet(entityLookup, "Titania", uranus, 3.527E21, 788400, 8.706234, 24.614, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(titania));
			var oberon = EntityUtility.CreatePlanet(entityLookup, "Oberon", uranus, 3.014E21, 761400, 13.463234, 283.088, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(oberon));

			var neptune = EntityUtility.CreatePlanet(entityLookup, "Neptune", sol, 1.0243E26, 2.4622E7, 60182, 256.228, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(neptune));
			var triton = EntityUtility.CreatePlanet(entityLookup, "Triton", neptune, 2.14E22, 1.3534E6, 5.876854, 264.775, false, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(triton));
			var nereid = EntityUtility.CreatePlanet(entityLookup, "Nereid", neptune, 3E19, 170000, 360.1362, 359.341, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(nereid));
			var proteus = EntityUtility.CreatePlanet(entityLookup, "Proteus", neptune, 4.4E19, 210000, 1.12231477, 117.050, true, gameData.Calendar);
			m_planets.Add(new PlanetViewModel(proteus));

			foreach (var planet in m_planets)
				planet.UpdateFromEntity(entityLookup);

			var ship1 = EntityUtility.CreateShip(entityLookup, "Discovery", new Point(m_gameServices.RandomNumberGenerator.NextDouble(-1E12, 1E12), m_gameServices.RandomNumberGenerator.NextDouble(-1E12, 1E12)));
			Ship = new ShipViewModel(ship1);
			m_ships.Add(Ship);

			foreach (var ship in m_ships)
				ship.UpdateFromEntity(entityLookup);
		}

		private static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(MainWindowViewModel));

		readonly GameServices m_gameServices;
		readonly SystemMapViewModel m_systemMap;
		readonly List<PlanetViewModel> m_planets;
		readonly List<ShipViewModel> m_ships;

		bool m_isGameStarted;
		string m_currentDate;
		PlanetViewModel m_planet;
		bool m_shouldRunAtFullSpeed;
		ShipViewModel m_ship;
		DispatcherOperation m_refreshOperation;
		string m_tokenStringTest;
	}
}
