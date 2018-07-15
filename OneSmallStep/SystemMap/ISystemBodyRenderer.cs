using System.Windows;
using System.Windows.Media;

namespace OneSmallStep.SystemMap
{
	public interface ISystemBodyRenderer
	{
		void Render(DrawingContext context, Point offset, double scale);
	}
}
