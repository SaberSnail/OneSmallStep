using System.Windows;
using System.Windows.Input;

namespace OneSmallStep.UI.EllipseTest
{
	public partial class EllipseTest : Window
	{
		public EllipseTest()
		{
			InitializeComponent();
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
				Renderer.IsEllipseActive = true;
			if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
				Renderer.IsRectangleActive = true;

			base.OnPreviewKeyDown(e);
		}

		protected override void OnPreviewKeyUp(KeyEventArgs e)
		{
			if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
				Renderer.IsEllipseActive = false;
			if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
				Renderer.IsRectangleActive = false;

			base.OnPreviewKeyDown(e);
		}
	}
}
