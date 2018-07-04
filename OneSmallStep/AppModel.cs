using System;
using System.Windows;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Logging;
using OneSmallStep.ECS;
using OneSmallStep.ECS.Components;
using OneSmallStep.MainWindow;

namespace OneSmallStep
{
	public sealed class AppModel
	{
		public static string OrganizationName
		{
			get { return "AStott Productions"; }
		}

		public static string ApplicationName
		{
			get { return "One Small Step"; }
		}

		public static AppModel Current => ((App) Application.Current).AppModel;

		public AppModel()
		{
			LogManager.Initialize(new ConsoleLogDestination(true));
			m_rng = new Random();
		}

		public event EventHandler StartupFinished;

		public Random Random
		{
			get { return m_rng; }
		}

		public MainWindowViewModel MainWindowViewModel
		{
			get { return m_mainWindowViewModel; }
		}

		public void Startup()
		{
			m_mainWindowViewModel = new MainWindowViewModel();
			m_entityManager = CreateEntityManager();

			StartupFinished.Raise(this);
		}

		public void Shutdown()
		{
		}

		private static EntityManager CreateEntityManager()
		{
			EntityManager entityManager = new EntityManager();

			entityManager.RegisterComponent<AgeComponent>();

			entityManager.SetStartupFinished();
			return entityManager;
		}

		readonly Random m_rng;
		MainWindowViewModel m_mainWindowViewModel;
		EntityManager m_entityManager;
	}
}
