using System;
using System.Windows;
using OneSmallStep.MainWindow;

namespace OneSmallStep
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			m_appModel = new AppModel();
			m_appModel.StartupFinished += AppModel_StartupFinished;
		}

		public AppModel AppModel
		{
			get { return m_appModel; }
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			m_appModel.Startup();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			m_appModel.Shutdown();

			base.OnExit(e);
		}

		private void AppModel_StartupFinished(object sender, EventArgs eventArgs)
		{
			new MainWindowView(m_appModel.MainWindowViewModel).Show();
		}

		readonly AppModel m_appModel;
	}
}
