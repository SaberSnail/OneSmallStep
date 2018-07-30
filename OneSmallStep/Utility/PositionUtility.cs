using System;
using System.Windows;

namespace OneSmallStep.Utility
{
	public static class PositionUtility
	{
		public static bool IsWithinOneMeter(this Point point, Point that)
		{
			return Math.Abs(point.X - that.X) < 1.0 && Math.Abs(point.Y - that.Y) < 1.0;
		}
	}
}
