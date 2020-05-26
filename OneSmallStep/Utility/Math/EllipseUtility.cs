using System;
using System.Collections.Generic;
using System.Windows;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Windows;
using static System.Math;

namespace OneSmallStep.Utility.Math
{
	public static class EllipseUtility
	{
		public static int OrbitsRendered = 0;
		public static int OrbitsSkipped = 0;

		public static bool ShouldRenderEllipseInRectangle(Rect viewRect, Point ellipseCenter, double semiMajorAxis, double semiMinorAxis)
		{
			var viewRadius = Sqrt(viewRect.Width * viewRect.Width + viewRect.Height * viewRect.Height) / 2.0;
			var viewCenter = new Point(viewRect.Left + (viewRect.Width / 2.0), viewRect.Top + (viewRect.Height / 2.0));
			var distanceBetweenCenters = ellipseCenter.DistanceTo(viewCenter);
			var maxEllipseRadius = semiMajorAxis;
			if (distanceBetweenCenters - maxEllipseRadius > viewRadius)
				return false;

			var centerEllipseRadius = semiMinorAxis;
			if ((distanceBetweenCenters + centerEllipseRadius < viewRadius) && (viewRadius < centerEllipseRadius))
				return false;

			return true;
		}

		public static Point GetPointAtAngle(Point ellipseCenter, double ellipseAngle, double semiMajorAxis, double semiMinorAxis, double angle)
		{
			var sinA = Sin(ellipseAngle);
			var cosA = Cos(ellipseAngle);
			var tanTheta = Tan(angle);

			var v1 = cosA + (tanTheta * sinA);
			var v2 = sinA - (tanTheta * cosA);

			var theta = MathUtility.NormalizeRadians(angle);
			var sign = (theta < (PI / 2.0)) || (theta > 3.0 * PI / 2.0) ? 1 : -1;

			var x = sign * semiMajorAxis * semiMinorAxis / Sqrt((semiMinorAxis * semiMinorAxis * v1 * v1) + (semiMajorAxis * semiMajorAxis * v2 * v2));

			var a = (semiMinorAxis * semiMinorAxis * sinA * sinA) + (semiMajorAxis * semiMajorAxis * cosA * cosA);
			var b = 2.0 * sinA * cosA * x * ((semiMinorAxis * semiMinorAxis) - (semiMajorAxis * semiMajorAxis));
			var c = (x * x * ((semiMinorAxis * semiMinorAxis * cosA * cosA) + (semiMajorAxis * semiMajorAxis * sinA * sinA))) - (semiMajorAxis * semiMajorAxis * semiMinorAxis * semiMinorAxis);

			sign = theta > PI ? -1 : 1;
			var discriminant = (b * b) - (4.0 * a * c);
			var y = (-b + (discriminant >= 0 ? (sign * Sqrt(discriminant)) : 0.0)) / (2.0 * a);

			return new Point(x + ellipseCenter.X, y + ellipseCenter.Y);
		}

		public static IReadOnlyList<(Point Intersection, double Slope)> FindIntersectionAndSlopeOfLineAndEllipse(Point linePoint1, Point linePoint2, bool isLineSegment, Point ellipseCenter, double majorRadius, double minorRadius, double ellipseRotation)
		{
			var sinA = Sin(ellipseRotation);
			var cosA = Cos(ellipseRotation);
			var majorSquared = majorRadius * majorRadius;
			var minorSquared = minorRadius * minorRadius;

			var dx = linePoint2.X - linePoint1.X;
			var dy = linePoint2.Y - linePoint1.Y;
			var x1 = linePoint1.X - ellipseCenter.X;
			var y1 = linePoint1.Y - ellipseCenter.Y;

			var e = (x1 * cosA) - (y1 * sinA);
			var f = (dx * cosA) - (dy * sinA);
			var g = (x1 * sinA) + (y1 * cosA);
			var h = (dx * sinA) + (dy * cosA);
			var a = ((f * f) / majorSquared) + ((h * h) / minorSquared);
			var b = ((2.0 * e * f) / majorSquared) + ((2.0 * g * h) / minorSquared);
			var c = ((e * e) / majorSquared) + ((g * g) / minorSquared) - 1.0;
			var values = MathUtility.SolveQuadratic(a, b, c);

			var points = new List<(Point Intersection, double Slope)>();
			foreach (var value in values)
			{
				if (!isLineSegment || ((value >= 0.0) && (value <= 1.0)))
				{
					var x = linePoint1.X + dx * value;
					var y = linePoint1.Y + dy * value;
					var slopeX = x - ellipseCenter.X;
					var slopeY = y - ellipseCenter.Y;
					var slope = -((majorSquared * slopeX * sinA * sinA) + (slopeY * (majorRadius - minorRadius) * (majorRadius + minorRadius) * sinA * cosA) + (minorSquared * slopeX * cosA * cosA)) / ((majorSquared * slopeY * cosA * cosA) + (slopeX * (majorRadius - minorRadius) * (majorRadius + minorRadius) * sinA * cosA) + (minorSquared * slopeY * sinA * sinA));
					points.Add((Intersection: new Point(x, y), Slope: slope));
				}
			}
			return points;
		}
	}
}
