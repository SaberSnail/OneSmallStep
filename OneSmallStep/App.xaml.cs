using System;
using System.Windows;
using OneSmallStep.UI.EllipseTest;
using OneSmallStep.UI.MainWindow;

namespace OneSmallStep
{
	public partial class App : Application
	{
		public App()
		{
			AppModel = new AppModel();
			AppModel.StartupFinished += AppModel_StartupFinished;
		}

		public AppModel AppModel { get; }

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			AppModel.Startup();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			AppModel.Shutdown();

			base.OnExit(e);
		}

		private void AppModel_StartupFinished(object sender, EventArgs eventArgs)
		{
			AppModel.StartupFinished -= AppModel_StartupFinished;

			new MainWindowView(AppModel.MainWindowViewModel).Show();

			new EllipseTest().Show();
		}
	}
}
