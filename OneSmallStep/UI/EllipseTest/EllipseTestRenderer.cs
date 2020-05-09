using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

			if (e.Delta > 0)
				Scale *= Math.Pow(1.15, Math.Abs(e.Delta * 0.01));
			else if (e.Delta < 0)
				Scale /= Math.Pow(1.15, Math.Abs(e.Delta * 0.01));

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
				context.DrawRectangle(null, IsRectangleActive ? s_activePen : s_rectanglePen, new Rect(adjustedLeft, adjustedTop, adjustedRight - adjustedLeft, adjustedBottom - adjustedTop));

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
				RenderIntersectionPoints(EllipseUtility.FindIntersectionAndSlopeOfLineAndEllipse(new Point(left, top), new Point(right, top), true, ellipseCenter, majorRadius, minorRadius, rotationRadians), scale, offset, context);
				RenderIntersectionPoints(EllipseUtility.FindIntersectionAndSlopeOfLineAndEllipse(new Point(right, top), new Point(right, bottom), true, ellipseCenter, majorRadius, minorRadius, rotationRadians), scale, offset, context);
				RenderIntersectionPoints(EllipseUtility.FindIntersectionAndSlopeOfLineAndEllipse(new Point(right, bottom), new Point(left, bottom), true, ellipseCenter, majorRadius, minorRadius, rotationRadians), scale, offset, context);
				RenderIntersectionPoints(EllipseUtility.FindIntersectionAndSlopeOfLineAndEllipse(new Point(left, bottom), new Point(left, top), true, ellipseCenter, majorRadius, minorRadius, rotationRadians), scale, offset, context);
			}
		}

		private void RenderIntersectionPoints(IEnumerable<(Point Intersection, double Slope)> points, double scale, Point offset, DrawingContext context)
		{
			foreach (var point in points)
			{
				Log.Info($"Intersection at {point.Intersection.X}, {point.Intersection.Y}; Slope = {point.Slope}");
				var x = (point.Intersection.X * scale) + offset.X;
				var y = (point.Intersection.Y * scale) + offset.Y;
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
		static readonly Pen s_activePen = new Pen(new SolidColorBrush(Colors.White).Frozen(), 1.5).Frozen();

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
