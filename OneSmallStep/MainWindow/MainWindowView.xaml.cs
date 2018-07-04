using System.Windows;

namespace OneSmallStep.MainWindow
{
	/// <summary>
	/// Interaction logic for MainWindowView.xaml
	/// </summary>
	public partial class MainWindowView : Window
	{
		public MainWindowView()
		{
			InitializeComponent();
		}

		public MainWindowView(MainWindowViewModel viewModel)
		{
			InitializeComponent();

			DataContext = viewModel;
			m_viewModel = viewModel;
		}

		readonly MainWindowViewModel m_viewModel;
	}
}
