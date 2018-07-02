using GoldenAnvil.Utility.Logging;

namespace OneSmallStep.MainWindow
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel()
		{
		}

		private static readonly ILogSource Log = LogManager.CreateLogSource(nameof(MainWindowViewModel));
	}
}
