using System;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Logging;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.ECS;
using OneSmallStep.UI.MainWindow;

namespace OneSmallStep
{
	public sealed class AppModel : NotifyPropertyChangedDispatcherBase
	{
		public AppModel()
		{
			LogManager.Initialize(new ConsoleLogDestination(true));

			CurrentTheme = new Uri(@"/UI/Themes/Default/Default.xaml", UriKind.Relative);
		}

		public event EventHandler StartupFinished;

		public MainWindowViewModel MainWindowViewModel => m_mainWindowViewModel;

		public GameServices GameServices => m_gameServices;

		public GameData GameData
		{
			get
			{
				VerifyAccess();
				return m_gameData;
			}
			set
			{
				SetPropertyField(value, ref m_gameData);
			}
		}

		public Uri CurrentTheme
		{
			get
			{
				VerifyAccess();
				return m_currentTheme;
			}
			set
			{
				if (SetPropertyField(value, ref m_currentTheme))
					Log.Info($"Changing theme to \"{m_currentTheme.OriginalString}\"");
			}
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

		private static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(AppModel));

		MainWindowViewModel m_mainWindowViewModel;
		GameServices m_gameServices;
		Uri m_currentTheme;
		GameData m_gameData;
	}
}
