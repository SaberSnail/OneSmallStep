using System.Windows;

namespace OneSmallStep.UI.MainWindow
{
	public partial class MainWindowView : Window
	{
		public MainWindowView(MainWindowViewModel viewModel)
		{
			ViewModel = viewModel;

			InitializeComponent();
		}

		public MainWindowViewModel ViewModel { get; }
	}
}
