using System.Windows;
using System.Windows.Input;
using OneSmallStep.ECS;

namespace OneSmallStep.UI.MainWindow
{
	/// <summary>
	/// Interaction logic for MainWindowView.xaml
	/// </summary>
	public partial class MainWindowView : Window
	{
		public MainWindowView(MainWindowViewModel viewModel)
		{
			ViewModel = viewModel;

			InitializeComponent();
		}

		public MainWindowViewModel ViewModel { get; }

		void OnGoToEntity(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.GoToEntity((EntityId) e.Parameter);
			e.Handled = true;
		}
	}
}
