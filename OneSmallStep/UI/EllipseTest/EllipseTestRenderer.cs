using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Logging;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.Utility;

namespace OneSmallStep.UI.EllipseTest
{
	public class EllipseTestRenderer : Control
	{
		public static readonly DependencyProperty LeftProperty = DependencyPropertyUtility<EllipseTestRenderer>.Register(x => x.Left, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, "-1");

		public string Left
		{
			get { return (string) GetValue(LeftProperty); }
			set { SetValue(LeftProperty, value); }
		}

		public static readonly DependencyProperty TopProperty = DependencyPropertyUtility<EllipseTestRenderer>.Register(x => x.Top, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, "-1");

		public string Top
		{
			get { return (string) GetValue(TopProperty); }
			set { SetValue(TopProperty, value); }
		}

		public static readonly DependencyProperty RightProperty = DependencyPropertyUtility<EllipseTestRenderer>.Register(x => x.Right, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, "1");

		public string Right
		{
			get { return (string) GetValue(RightProperty); }
			set { SetValue(RightProperty, value); }
		}

		public static readonly DependencyProperty BottomProperty = DependencyPropertyUtility<EllipseTestRenderer>.Register(x => x.Bottom, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, "1");

		public string Bottom
		{
			get { return (string) GetValue(BottomProperty); }
			set { SetValue(BottomProperty, value); }
		}

		public static readonly DependencyProperty EllipseCenterXProperty = DependencyPropertyUtility<EllipseTestRenderer>.Register(x => x.EllipseCenterX, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, "0");

		public string EllipseCenterX
		{
			get { return (string) GetValue(EllipseCenterXProperty); }
			set { SetValue(EllipseCenterXProperty, value); }
		}

		public static readonly DependencyProperty EllipseCenterYProperty = DependencyPropertyUtility<EllipseTestRenderer>.Register(x => x.EllipseCenterY, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, "0");

		public string EllipseCenterY
		{
			get { return (string) GetValue(EllipseCenterYProperty); }
			set { SetValue(EllipseCenterYProperty, value); }
		}

		public static readonly DependencyProperty MajorRadiusProperty = DependencyPropertyUtility<EllipseTestRenderer>.Register(x => x.MajorRadius, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, "1.2");

		public string MajorRadius
		{
			get { return (string) GetValue(MajorRadiusProperty); }
			set { SetValue(MajorRadiusProperty, value); }
		}

		public static readonly DependencyProperty MinorRadiusProperty = DependencyPropertyUtility<EllipseTestRenderer>.Register(x => x.MinorRadius, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, "0.8");

		public string MinorRadius
		{
			get { return (string) GetValue(MinorRadiusProperty); }
			set { SetValue(MinorRadiusProperty, value); }
		}

		public static readonly DependencyProperty EllipseRotationProperty = DependencyPropertyUtility<EllipseTestRenderer>.Register(x => x.EllipseRotation, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, "0");

		public string EllipseRotation
		{
			get { return (string) GetValue(EllipseRotationProperty); }
			set { SetValue(EllipseRotationProperty, value); }
		}

		protected override void OnRender(DrawingContext context)
		{
			base.OnRender(context);

			var viewRect = new Rect(0, 0, ActualWidth, ActualHeight);
			using (context.ScopedClip(new RectangleGeometry(viewRect)))
			{
				context.DrawRectangle(s_backgroundBrush, null, viewRect);

				if (!double.TryParse(Left, out var left))
					return;
				if (!double.TryParse(Top, out var top))
					return;
				if (!double.TryParse(Right, out var right))
					return;
				if (!double.TryParse(Bottom, out var bottom))
					return;

				var largestX = Math.Max(Math.Abs(top), Math.Abs(bottom));
				var xScale = ((ActualWidth / 2.0) - c_margin) / largestX;
				var largestY = Math.Max(Math.Abs(left), Math.Abs(right));
				var yScale = ((ActualHeight / 2.0) - c_margin) / largestY;
				var scale = Math.Min(xScale, yScale);

				var adjustedLeft = (left * scale) + (ActualWidth / 2.0);
				var adjustedTop = (top * scale) + (ActualHeight / 2.0);
				var adjustedRight = (right * scale) + (ActualWidth / 2.0);
				var adjustedBottom = (bottom * scale) + (ActualHeight / 2.0);
				context.DrawRectangle(null, s_rectanglePen, new Rect(adjustedLeft, adjustedTop, adjustedRight - adjustedLeft, adjustedBottom - adjustedTop));

				if (!double.TryParse(EllipseCenterX, out var ellipseCenterX))
					return;
				if (!double.TryParse(EllipseCenterY, out var ellipseCenterY))
					return;
				if (!double.TryParse(MajorRadius, out var majorRadius))
					return;
				if (!double.TryParse(MinorRadius, out var minorRadius))
					return;
				if (!double.TryParse(EllipseRotation, out var ellipseRotation))
					return;

				var adjustedEllipseCenterX = (ellipseCenterX * scale) + (ActualWidth / 2.0);
				var adjustedEllipseCenterY = (ellipseCenterY * scale) + (ActualHeight / 2.0);
				var adjustedMajorRadius = majorRadius * scale;
				var adjustedMinorRadius = minorRadius * scale;
				using (context.ScopedTransform(new TranslateTransform(adjustedEllipseCenterX, adjustedEllipseCenterY)))
				using (context.ScopedTransform(new RotateTransform(ellipseRotation, 0.0, 0.0)))
					context.DrawEllipse(null, s_orbitPen, new Point(), adjustedMajorRadius, adjustedMinorRadius);

				context.DrawEllipse(s_centerBrush, null, new Point(adjustedEllipseCenterX, adjustedEllipseCenterY), c_markerRadius, c_markerRadius);

				var rotationRadians = MathUtility.DegreesToRadians(-ellipseRotation);
				var ellipseCenter = new Point(ellipseCenterX, ellipseCenterY);
				RenderIntersectionPoints(EllipseUtility.FindIntersectionAndSlopeOfLineAndEllipse(new Point(left, top), new Point(right, top), true, ellipseCenter, majorRadius, minorRadius, rotationRadians), scale, context);
				RenderIntersectionPoints(EllipseUtility.FindIntersectionAndSlopeOfLineAndEllipse(new Point(right, top), new Point(right, bottom), true, ellipseCenter, majorRadius, minorRadius, rotationRadians), scale, context);
				RenderIntersectionPoints(EllipseUtility.FindIntersectionAndSlopeOfLineAndEllipse(new Point(right, bottom), new Point(left, bottom), true, ellipseCenter, majorRadius, minorRadius, rotationRadians), scale, context);
				RenderIntersectionPoints(EllipseUtility.FindIntersectionAndSlopeOfLineAndEllipse(new Point(left, bottom), new Point(left, top), true, ellipseCenter, majorRadius, minorRadius, rotationRadians), scale, context);
			}
		}

		private void RenderIntersectionPoints(IEnumerable<(Point Intersection, double Slope)> points, double scale, DrawingContext context)
		{
			foreach (var point in points)
			{
				Log.Info($"Intersection at {point.Intersection.X}, {point.Intersection.Y}; Slope = {point.Slope}");
				var x = (point.Intersection.X * scale) + (ActualWidth / 2.0);
				var y = (point.Intersection.Y * scale) + (ActualHeight / 2.0);
				var intersectionPoint = new Point(x, y);

				var slopeVector = new Vector(1, point.Slope);
				slopeVector.Normalize();
				slopeVector *= 25.0;
				var p1 = intersectionPoint - slopeVector;
				var p2 = intersectionPoint + slopeVector;
				context.DrawLine(s_tangentPen, p1, p2);

				context.DrawEllipse(s_intersectionBrush, null, intersectionPoint, c_markerRadius, c_markerRadius);
			}
		}

		static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(EllipseTestRenderer));

		static readonly Brush s_backgroundBrush = new SolidColorBrush(Colors.Black).Frozen();
		static readonly Pen s_rectanglePen = new Pen(new SolidColorBrush(Colors.LightGray).Frozen(), 1.0).Frozen();
		static readonly Pen s_orbitPen = new Pen(new SolidColorBrush(Colors.LightGray).Frozen(), 1.0).Frozen();
		static readonly Brush s_intersectionBrush = new SolidColorBrush(Colors.Red).Frozen();
		static readonly Brush s_centerBrush = new SolidColorBrush(Colors.LawnGreen).Frozen();
		static readonly Pen s_tangentPen = new Pen(new SolidColorBrush(Colors.DarkGray).Frozen(), 1.0).Frozen();

		const double c_margin = 50.0;
		const double c_markerRadius = 4.0;
	}
}
