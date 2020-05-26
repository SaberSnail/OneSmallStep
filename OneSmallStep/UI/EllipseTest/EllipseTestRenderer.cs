using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Logging;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.Utility;
using OneSmallStep.Utility.Math;

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

		public static readonly DependencyProperty ScaleProperty = DependencyPropertyUtility<EllipseTestRenderer>.Register(x => x.Scale, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, 1.0);

		public double Scale
		{
			get { return (double) GetValue(ScaleProperty); }
			set { SetValue(ScaleProperty, value); }
		}

		public static readonly DependencyProperty CenterProperty = DependencyPropertyUtility<EllipseTestRenderer>.Register(x => x.Center, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender);

		public Point Center
		{
			get { return (Point) GetValue(CenterProperty); }
			set { SetValue(CenterProperty, value); }
		}

		public static readonly DependencyProperty IsEllipseActiveProperty = DependencyPropertyUtility<EllipseTestRenderer>.Register(x => x.IsEllipseActive, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender);

		public bool IsEllipseActive
		{
			get { return (bool) GetValue(IsEllipseActiveProperty); }
			set { SetValue(IsEllipseActiveProperty, value); }
		}

		public static readonly DependencyProperty IsRectangleActiveProperty = DependencyPropertyUtility<EllipseTestRenderer>.Register(x => x.IsRectangleActive, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender);

		public bool IsRectangleActive
		{
			get { return (bool) GetValue(IsRectangleActiveProperty); }
			set { SetValue(IsRectangleActiveProperty, value); }
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);

			if (IsEllipseActive)
			{
				if (double.TryParse(MajorRadius, out var majorRadius) &&
					double.TryParse(MinorRadius, out var minorRadius))
				{
					if (e.Delta > 0)
					{
						majorRadius *= Math.Pow(1.05, Math.Abs(e.Delta * 0.01));
						minorRadius *= Math.Pow(1.05, Math.Abs(e.Delta * 0.01));
					}
					else if (e.Delta < 0)
					{
						majorRadius /= Math.Pow(1.05, Math.Abs(e.Delta * 0.01));
						minorRadius /= Math.Pow(1.05, Math.Abs(e.Delta * 0.01));
					}
					MajorRadius = majorRadius.ToString();
					MinorRadius = minorRadius.ToString();
				}
			}

			if (!IsEllipseActive && !IsRectangleActive)
			{
				if (e.Delta > 0)
					Scale *= Math.Pow(1.15, Math.Abs(e.Delta * 0.01));
				else if (e.Delta < 0)
					Scale /= Math.Pow(1.15, Math.Abs(e.Delta * 0.01));
			}

			e.Handled = true;
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			m_mouseDownPosition = e.GetPosition(this);
			m_mouseDownCenter = Center;
			m_mouseDownEllipseCenterX = EllipseCenterX;
			m_mouseDownEllipseCenterY = EllipseCenterY;
			m_mouseDownRectangleTop = Top;
			m_mouseDownRectangleLeft = Left;
			m_mouseDownRectangleBottom = Bottom;
			m_mouseDownRectangleRight = Right;
			m_isMouseDown = true;
			CaptureMouse();

			e.Handled = true;
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			m_isDragging = false;
			m_isMouseDown = false;
			ReleaseMouseCapture();

			e.Handled = true;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (m_isMouseDown)
			{
				var currentPosition = e.GetPosition(this);
				if (!m_isDragging && Math.Max(Math.Abs(currentPosition.X - m_mouseDownPosition.X), Math.Abs(currentPosition.X - m_mouseDownPosition.X)) > 4.0)
					m_isDragging = true;

				if (m_isDragging)
				{
					var scale = Scale * Math.Min(ActualWidth / 2.0, ActualHeight / 2.0);

					if (IsEllipseActive)
					{
						if (double.TryParse(EllipseCenterX, out var ellipseCenterX) &&
							double.TryParse(EllipseCenterY, out var ellipseCenterY) &&
							double.TryParse(m_mouseDownEllipseCenterX, out var originalEllipseCenterX) &&
							double.TryParse(m_mouseDownEllipseCenterY, out var originalEllipseCenterY))
						{
							ellipseCenterX = originalEllipseCenterX - ((m_mouseDownPosition.X - currentPosition.X) / scale);
							ellipseCenterY = originalEllipseCenterY - ((m_mouseDownPosition.Y - currentPosition.Y) / scale);
							EllipseCenterX = ellipseCenterX.ToString();
							EllipseCenterY = ellipseCenterY.ToString();
						}
					}
					if (IsRectangleActive)
					{
						if (double.TryParse(Top, out var rectTop) &&
							double.TryParse(Left, out var rectLeft) &&
							double.TryParse(Bottom, out var rectBottom) &&
							double.TryParse(Right, out var rectRight) && 
							double.TryParse(m_mouseDownRectangleTop, out var originalRectTop) &&
							double.TryParse(m_mouseDownRectangleLeft, out var originalRectLeft) &&
							double.TryParse(m_mouseDownRectangleBottom, out var originalRectBottom) &&
							double.TryParse(m_mouseDownRectangleRight, out var originalRectRight))
						{
							rectTop = originalRectTop - ((m_mouseDownPosition.Y - currentPosition.Y) / scale);
							rectLeft = originalRectLeft - ((m_mouseDownPosition.X - currentPosition.X) / scale);
							rectBottom = originalRectBottom - ((m_mouseDownPosition.Y - currentPosition.Y) / scale);
							rectRight = originalRectRight - ((m_mouseDownPosition.X - currentPosition.X) / scale);
							Top = rectTop.ToString();
							Left = rectLeft.ToString();
							Bottom = rectBottom.ToString();
							Right = rectRight.ToString();
						}
					}
					if (!IsEllipseActive && !IsRectangleActive)
					{
						Center = new Point(
							m_mouseDownCenter.X + ((m_mouseDownPosition.X - currentPosition.X) / scale),
							m_mouseDownCenter.Y + ((m_mouseDownPosition.Y - currentPosition.Y) / scale)
						);
					}
				}

				e.Handled = true;
			}
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

				var centerPoint = new Point(viewRect.Width / 2.0, viewRect.Height / 2.0);
				var scale = Scale * Math.Min(centerPoint.X, centerPoint.Y);
				var offset = new Point(centerPoint.X - (Center.X * scale), centerPoint.Y - (Center.Y * scale));

				var adjustedLeft = (left * scale) + offset.X;
				var adjustedTop = (top * scale) + offset.Y;
				var adjustedRight = (right * scale) + offset.X;
				var adjustedBottom = (bottom * scale) + offset.Y;
				var adjustedRectangle = new Rect(adjustedLeft, adjustedTop, adjustedRight - adjustedLeft, adjustedBottom - adjustedTop);
				context.DrawRectangle(null, IsRectangleActive ? s_activePen : s_rectanglePen, adjustedRectangle);

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

				var adjustedEllipseCenterX = (ellipseCenterX * scale) + offset.X;
				var adjustedEllipseCenterY = (ellipseCenterY * scale) + offset.Y;
				var adjustedMajorRadius = majorRadius * scale;
				var adjustedMinorRadius = minorRadius * scale;
				using (context.ScopedTransform(new TranslateTransform(adjustedEllipseCenterX, adjustedEllipseCenterY)))
				using (context.ScopedTransform(new RotateTransform(ellipseRotation, 0.0, 0.0)))
					context.DrawEllipse(null, IsEllipseActive ? s_activePen : s_orbitPen, new Point(), adjustedMajorRadius, adjustedMinorRadius);

				context.DrawEllipse(s_centerBrush, null, new Point(adjustedEllipseCenterX, adjustedEllipseCenterY), c_markerRadius, c_markerRadius);

				var rotationRadians = MathUtility.DegreesToRadians(-ellipseRotation);
				var ellipseCenter = new Point(ellipseCenterX, ellipseCenterY);

				var leftPoints = NormalizeIntersectionPoints(EllipseUtility.FindIntersectionAndSlopeOfLineAndEllipse(new Point(left, bottom), new Point(left, top), true, ellipseCenter, majorRadius, minorRadius, rotationRadians), scale, offset);
				RenderIntersectionPoints(leftPoints, context);

				var topPoints = NormalizeIntersectionPoints(EllipseUtility.FindIntersectionAndSlopeOfLineAndEllipse(new Point(left, top), new Point(right, top), true, ellipseCenter, majorRadius, minorRadius, rotationRadians), scale, offset);
				RenderIntersectionPoints(topPoints, context);

				var rightPoints = NormalizeIntersectionPoints(EllipseUtility.FindIntersectionAndSlopeOfLineAndEllipse(new Point(right, top), new Point(right, bottom), true, ellipseCenter, majorRadius, minorRadius, rotationRadians), scale, offset);
				RenderIntersectionPoints(rightPoints, context);

				var bottomPoints = NormalizeIntersectionPoints(EllipseUtility.FindIntersectionAndSlopeOfLineAndEllipse(new Point(right, bottom), new Point(left, bottom), true, ellipseCenter, majorRadius, minorRadius, rotationRadians), scale, offset);
				RenderIntersectionPoints(bottomPoints, context);

				var normalizedEllipseCenter = new Point((ellipseCenter.X * scale) + offset.X, (ellipseCenter.Y * scale) + offset.Y);
				RenderEstimates(leftPoints, topPoints, rightPoints, bottomPoints, normalizedEllipseCenter, MathUtility.DegreesToRadians(ellipseRotation), adjustedMajorRadius, adjustedMinorRadius, adjustedRectangle, context);
			}
		}

		private IReadOnlyList<(Point Intersection, double Slope)> NormalizeIntersectionPoints(IReadOnlyList<(Point Intersection, double Slope)> points, double scale, Point offset)
		{
			return points
				.Select(point =>
				{
					Log.Info($"Intersection at {point.Intersection.X}, {point.Intersection.Y}; Slope = {point.Slope}");
					var x = (point.Intersection.X * scale) + offset.X;
					var y = (point.Intersection.Y * scale) + offset.Y;
					return (Intersection: new Point(x, y), Slope: point.Slope);
				})
				.ToList()
				.AsReadOnly();
		}

		private void RenderIntersectionPoints(IReadOnlyList<(Point Intersection, double Slope)> points, DrawingContext context)
		{
			foreach (var point in points)
			{
				var slopeVector = new Vector(1, point.Slope);
				slopeVector.Normalize();
				slopeVector *= 25.0;
				var p1 = point.Intersection - slopeVector;
				var p2 = point.Intersection + slopeVector;
				context.DrawLine(s_tangentPen, p1, p2);

				context.DrawEllipse(s_intersectionBrush, null, point.Intersection, c_markerRadius, c_markerRadius);
			}
		}

		private void RenderEstimates(IReadOnlyList<(Point Intersection, double Slope)> leftPoints, IReadOnlyList<(Point Intersection, double Slope)> topPoints, IReadOnlyList<(Point Intersection, double Slope)> rightPoints, IReadOnlyList<(Point Intersection, double Slope)> bottomPoints, Point ellipseCenter, double ellipseAngle, double semiMajorAxis, double semiMinorAxis, Rect rectangle, DrawingContext context)
		{
			var points = leftPoints.Select(p => p.Intersection).OrderBy(p => p.Y)
				.Concat(bottomPoints.Select(p => p.Intersection).OrderBy(p => p.X))
				.Concat(rightPoints.Select(p => p.Intersection).OrderByDescending(p => p.Y))
				.Concat(topPoints.Select(p => p.Intersection).OrderByDescending(p => p.X))
				.AsReadOnlyList();

			if (points.Count <= 1)
				return;

			var a1 = ellipseCenter.AngleTo(points[0]);
			var a2 = ellipseCenter.AngleTo(points[1]);
			while (a1 < a2)
				a1 += 2.0 * Math.PI;
			var midA = (a1 + a2) / 2.0;
			var midPoint = EllipseUtility.GetPointAtAngle(ellipseCenter, ellipseAngle, semiMajorAxis, semiMinorAxis, midA);

			if (!rectangle.Contains(midPoint))
			{
				points = points
					.Append(points[0])
					.Skip(1)
					.AsReadOnlyList();
			}

			for (int i = 0; i < points.Count; i += 2)
			{
				var angle1 = ellipseCenter.AngleTo(points[i]);
				var angle2 = ellipseCenter.AngleTo(points[i+1]);
				while (angle1 < angle2)
					angle1 += 2.0 * Math.PI;
				var midAngle = (angle1 + angle2) / 2.0;
				var midPointOnEllipse = EllipseUtility.GetPointAtAngle(ellipseCenter, ellipseAngle, semiMajorAxis, semiMinorAxis, midAngle);

				if (i == 0)
				{
					context.DrawLine(s_tangentPen, ellipseCenter, new Point(semiMajorAxis * Math.Cos(midAngle) + ellipseCenter.X, semiMajorAxis * Math.Sin(midAngle) + ellipseCenter.Y));
					context.DrawLine(s_angle1Pen, ellipseCenter, new Point(semiMajorAxis * Math.Cos(angle1) + ellipseCenter.X, semiMajorAxis * Math.Sin(angle1) + ellipseCenter.Y));
					context.DrawLine(s_angle2Pen, ellipseCenter, new Point(semiMajorAxis * Math.Cos(angle2) + ellipseCenter.X, semiMajorAxis * Math.Sin(angle2) + ellipseCenter.Y));
					context.DrawEllipse(s_intersectionBrush, null, midPointOnEllipse, c_markerRadius, c_markerRadius);
				}

				var control1 = points[i] + ((midPointOnEllipse - points[i]) / 2.0);
				var control2 = points[i+1] + ((midPointOnEllipse - points[i+1]) / 2.0);
				var geometry = new PathGeometry(new PathFigure[] {
					new PathFigure(points[i], new PathSegment[] {
						new BezierSegment(control1, control2, points[i+1], true)
					}, false),
				});
				context.DrawGeometry(null, s_estimatePen, geometry);
			}
		}

		static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(EllipseTestRenderer));

		static readonly Brush s_backgroundBrush = new SolidColorBrush(Colors.Black).Frozen();
		static readonly Pen s_rectanglePen = new Pen(new SolidColorBrush(Colors.LightGray).Frozen(), 1.0).Frozen();
		static readonly Pen s_orbitPen = new Pen(new SolidColorBrush(Colors.LightGray).Frozen(), 1.0).Frozen();
		static readonly Brush s_intersectionBrush = new SolidColorBrush(Colors.Red).Frozen();
		static readonly Brush s_centerBrush = new SolidColorBrush(Colors.LawnGreen).Frozen();
		static readonly Pen s_tangentPen = new Pen(new SolidColorBrush(Colors.DarkGray).Frozen(), 1.0).Frozen();
		static readonly Pen s_activePen = new Pen(new SolidColorBrush(Colors.White).Frozen(), 1.5).Frozen();
		static readonly Pen s_estimatePen = new Pen(new SolidColorBrush(Colors.Green).Frozen(), 1.0).Frozen();
		static readonly Pen s_angle1Pen = new Pen(new SolidColorBrush(Colors.Blue).Frozen(), 1.0).Frozen();
		static readonly Pen s_angle2Pen = new Pen(new SolidColorBrush(Colors.Yellow).Frozen(), 1.0).Frozen();

		const double c_margin = 50.0;
		const double c_markerRadius = 4.0;

		Point m_mouseDownPosition;
		bool m_isDragging;
		Point m_mouseDownCenter;
		string m_mouseDownEllipseCenterX;
		string m_mouseDownEllipseCenterY;
		string m_mouseDownRectangleTop;
		string m_mouseDownRectangleLeft;
		string m_mouseDownRectangleBottom;
		string m_mouseDownRectangleRight;
		bool m_isMouseDown;
	}
}
