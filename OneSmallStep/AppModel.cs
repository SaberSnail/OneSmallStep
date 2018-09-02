using System;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Logging;
using OneSmallStep.ECS;
using OneSmallStep.MainWindow;

namespace OneSmallStep
{
	public sealed class AppModel
	{
		public AppModel()
		{
			LogManager.Initialize(new ConsoleLogDestination(true));
		}

		public event EventHandler StartupFinished;

		public MainWindowViewModel MainWindowViewModel
		{
			get { return m_mainWindowViewModel; }
		}

		public void Startup()
		{
			m_gameServices = new GameServices();

			m_mainWindowViewModel = new MainWindowViewModel(m_gameServices);

			StartupFinished.Raise(this);
		}

		public void Shutdown()
		{
			m_gameServices.Dispose();
		}

		MainWindowViewModel m_mainWindowViewModel;
		GameServices m_gameServices;
	}
}
