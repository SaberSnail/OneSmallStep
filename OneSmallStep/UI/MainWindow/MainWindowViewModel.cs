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
				return m_status;
			}
			private set
			{
				SetPropertyField(value, ref m_status);
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
					var newTargetId = m_planets[m_gameServices.RandomNumberGenerator.Next(0, m_planets.Count)].EntityId;
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
			var rng = m_gameServices.RandomNumberGenerator;

			var planetViewModels = SystemDataFileUtility.LoadEntities("Data\\SolSystem.txt", entityLookup, rng)
				.Select(x => new PlanetViewModel(x));
			m_planets.AddRange(planetViewModels);

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
		string m_status;
	}
}
