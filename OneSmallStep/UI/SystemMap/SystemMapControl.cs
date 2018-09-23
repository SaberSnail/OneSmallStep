using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.Utility;

namespace OneSmallStep.UI.SystemMap
{
	public class SystemMapControl : Canvas
	{
		public static readonly DependencyProperty CenterProperty = DependencyPropertyUtility<SystemMapControl>.Register(x => x.Center, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender);

		public Point Center
		{
			get { return (Point) GetValue(CenterProperty); }
			set { SetValue(CenterProperty, value); }
		}

		public static readonly DependencyProperty ScaleProperty = DependencyPropertyUtility<SystemMapControl>.Register(x => x.Scale, OnScaleChanged, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, 1.0);

		public double Scale
		{
			get { return (double) GetValue(ScaleProperty); }
			set { SetValue(ScaleProperty, value); }
		}

		public static readonly DependencyProperty DateProperty = DependencyPropertyUtility<SystemMapControl>.Register(x => x.Date, null, FrameworkPropertyMetadataOptions.AffectsRender);

		public string Date
		{
			get { return (string) GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}

		public static readonly DependencyProperty BodiesProperty = DependencyPropertyUtility<SystemMapControl>.Register(x => x.Bodies, null, FrameworkPropertyMetadataOptions.AffectsRender, new List<ISystemBodyRenderer>());

		public IReadOnlyList<ISystemBodyRenderer> Bodies
		{
			get { return (IReadOnlyList<ISystemBodyRenderer>) GetValue(BodiesProperty); }
			set { SetValue(BodiesProperty, value); }
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);

			if (e.Delta > 0)
				Scale *= Math.Pow(1.1, Math.Abs(e.Delta * 0.01));
			else if (e.Delta < 0)
				Scale /= Math.Pow(1.1, Math.Abs(e.Delta * 0.01));

			e.Handled = true;
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			m_mouseDownPosition = e.GetPosition(this);
			m_mouseDownCenter = Center;
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
					Center = new Point(
						m_mouseDownCenter.X + ((m_mouseDownPosition.X - currentPosition.X) / scale),
						m_mouseDownCenter.Y + ((m_mouseDownPosition.Y - currentPosition.Y) / scale)
					);
				}

				e.Handled = true;
			}
		}

		protected override void OnRender(DrawingContext context)
		{
			base.OnRender(context);

			var viewRect = new Rect(0, 0, ActualWidth, ActualHeight);
			var background = (Brush) FindResource("SystemMapBackgroundBrush");
			context.DrawRectangle(background, null, viewRect);

			var centerPoint = new Point(viewRect.Width / 2.0, viewRect.Height / 2.0);
			var scale = Scale * Math.Min(centerPoint.X, centerPoint.Y);
			var offset = new Point(centerPoint.X - (Center.X * scale), centerPoint.Y - (Center.Y * scale));
			using (context.ScopedClip(new RectangleGeometry(viewRect)))
			{
				foreach (var body in Bodies.EmptyIfNull())
					body.Render(context, offset, scale);

				DrawScale(context, viewRect);
			}
		}

		private void DrawScale(DrawingContext context, Rect viewRect)
		{
			var scalePen = (Pen) FindResource("SystemMapScalePen");
			var scaleWidth = (double) FindResource("SystemMapScaleWidth");
			context.DrawLine(scalePen, new Point(viewRect.Right - 8, viewRect.Bottom - 8), new Point(viewRect.Right - scaleWidth - 8, viewRect.Bottom - 8));
			if (m_scaleText != null)
			context.DrawText(m_scaleText, new Point(viewRect.Right - 58 - (m_scaleText.Width / 2.0), viewRect.Bottom - 8 - m_scaleText.Height));
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			var actualSize = base.ArrangeOverride(arrangeSize);
			RefreshScaleText(actualSize);
			return actualSize;
		}

		private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (SystemMapControl) d;
			control.RefreshScaleText(new Size(control.ActualWidth, control.ActualHeight));
		}

		private void RefreshScaleText(Size actualSize)
		{
			var viewRect = new Rect(new Point(), actualSize);
			var centerPoint = new Point(viewRect.Width / 2.0, viewRect.Height / 2.0);
			var scale = Scale * Math.Min(centerPoint.X, centerPoint.Y);

			var textBrush = (Brush) FindResource("SystemMapScaleTextBrush");
			var scaleWidth = (double) FindResource("SystemMapScaleWidth");
			m_scaleText = new FormattedText(FormatUtility.RenderDistance(scaleWidth / scale), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 12, textBrush, 1.0);
		}

		FormattedText m_scaleText;
		Point m_mouseDownPosition;
		bool m_isDragging;
		Point m_mouseDownCenter;
		bool m_isMouseDown;
	}
}
