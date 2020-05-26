using System.Windows;
using static System.Math;

namespace OneSmallStep.Utility
{
	public static class PositionUtility
	{
		public static bool IsWithinOneMeter(this Point point, Point that)
		{
			return Abs(point.X - that.X) < 1.0 && Abs(point.Y - that.Y) < 1.0;
		}
	}
}
