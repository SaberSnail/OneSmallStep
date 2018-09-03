using System.Windows;
using System.Windows.Media;

namespace OneSmallStep.UI.SystemMap
{
	public interface ISystemBodyRenderer
	{
		void Render(DrawingContext context, Point offset, double scale);
	}
}
