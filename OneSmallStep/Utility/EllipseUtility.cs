using System;
using System.Collections.Generic;
using System.Windows;
using GoldenAnvil.Utility;

namespace OneSmallStep.Utility
{
	public static class EllipseUtility
	{
		public static IReadOnlyList<Point> FindIntersectionOfLineAndEllipse(Point linePoint1, Point linePoint2, bool isLineSegment, Point ellipseCenter, double majorRadius, double minorRadius, double ellipseRotation)
		{
			var sinA = Math.Sin(ellipseRotation);
			var cosA = Math.Cos(ellipseRotation);
			var dx = linePoint2.X - linePoint1.X;
			var dy = linePoint2.Y - linePoint1.Y;
			var x1 = linePoint1.X - ellipseCenter.X;
			var y1 = linePoint1.Y - ellipseCenter.Y;
			var e = (x1 * cosA) - (y1 * sinA);
			var f = (dx * cosA) - (dy * sinA);
			var g = (x1 * sinA) + (y1 * cosA);
			var h = (dx * sinA) + (dy * cosA);
			var majorSquared = majorRadius * majorRadius;
			var minorSquared = minorRadius * minorRadius;
			var a = ((f * f) / majorSquared) + ((h * h) / minorSquared);
			var b = ((2.0 * e * f) / majorSquared) + ((2.0 * g * h) / minorSquared);
			var c = ((e * e) / majorSquared) + ((g * g) / minorSquared) - 1.0;
			var values = MathUtility.SolveQuadratic(a, b, c);

			var points = new List<Point>();
			foreach (var value in values)
			{
				if (!isLineSegment || ((value >= 0.0) && (value <= 1.0)))
				{
					var x = linePoint1.X + dx * value;
					var y = linePoint1.Y + dy * value;
					points.Add(new Point(x, y));
				}
			}
			return points;
		}
	}
}
