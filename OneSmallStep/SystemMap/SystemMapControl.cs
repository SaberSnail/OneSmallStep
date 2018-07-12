using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.ECS;
using OneSmallStep.ECS.Components;

namespace OneSmallStep.SystemMap
{
	public class SystemMapControl : Canvas
	{
		public static readonly DependencyProperty CenterProperty = DependencyPropertyUtility<SystemMapControl>.Register(x => x.Center, OnInvalidatingPropertyChanged);

		public Point Center
		{
			get { return (Point) GetValue(CenterProperty); }
			set { SetValue(CenterProperty, value); }
		}

		public static readonly DependencyProperty ScaleProperty = DependencyPropertyUtility<SystemMapControl>.Register(x => x.Scale, OnInvalidatingPropertyChanged, 1.0);

		public double Scale
		{
			get { return (double) GetValue(ScaleProperty); }
			set { SetValue(ScaleProperty, value); }
		}

		public static readonly DependencyProperty DateProperty = DependencyPropertyUtility<SystemMapControl>.Register(x => x.Date, OnInvalidatingPropertyChanged);

		public string Date
		{
			get { return (string) GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}

		public static readonly DependencyProperty EntitiesProperty = DependencyPropertyUtility<SystemMapControl>.Register(x => x.Entities, OnInvalidatingPropertyChanged, new List<Entity>());

		public IReadOnlyList<Entity> Entities
		{
			get { return (IReadOnlyList<Entity>) GetValue(EntitiesProperty); }
			set { SetValue(EntitiesProperty, value); }
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
						m_mouseDownCenter.X + ((currentPosition.X - m_mouseDownPosition.X) / scale),
						m_mouseDownCenter.Y + ((currentPosition.Y - m_mouseDownPosition.Y) / scale)
					);
				}

				e.Handled = true;
			}
		}

		protected override void OnRender(DrawingContext context)
		{
			base.OnRender(context);

			var actualRect = new Rect(0, 0, ActualWidth, ActualHeight);
			context.DrawRectangle(s_background, null, actualRect);

			var centerPoint = new Point(actualRect.Width / 2.0, actualRect.Height / 2.0);
			var scale = Scale * Math.Min(centerPoint.X, centerPoint.Y);
			var translate = new TranslateTransform(
				centerPoint.X + (Center.X * scale),
				centerPoint.Y + (Center.Y * scale)
				);
			using (context.ScopedClip(new RectangleGeometry(actualRect)))
			using (context.ScopedTransform(translate))
			{
				foreach (var entity in Entities.EmptyIfNull())
				{
					var unpoweredBody = entity.GetComponent<UnpoweredAstronomicalBodyComponent>();
					if (unpoweredBody != null)
					{
						var position = unpoweredBody.GetAbsolutePosition();
						var renderAt = new Point(position.X * scale, position.Y * scale);
						context.DrawEllipse(s_bodyBrush, s_bodyPen, renderAt, 4, 4);
						continue;
					}

					var poweredBody = entity.GetComponent<PoweredAstronomicalBodyComponent>();
					if (poweredBody != null)
					{
						var position = poweredBody.AbsolutePosition;
						var renderAt = new Point(position.X * scale, position.Y * scale);
						context.DrawLine(s_bodyPen, new Point(renderAt.X - 4, renderAt.Y), new Point(renderAt.X + 4, renderAt.Y));
						context.DrawLine(s_bodyPen, new Point(renderAt.X, renderAt.Y - 4), new Point(renderAt.X, renderAt.Y + 4));
						if (poweredBody.TargetPoint.HasValue)
						{
							var renderTo = new Point(poweredBody.TargetPoint.Value.X * scale, poweredBody.TargetPoint.Value.Y * scale);
							context.DrawLine(s_pathPen, renderAt, renderTo);
						}
						continue;
					}
				}
			}
		}

		private static void OnInvalidatingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((SystemMapControl) d).InvalidateVisual();
		}

		static readonly Brush s_background = new SolidColorBrush(Colors.Black).Frozen();
		static readonly Brush s_bodyBrush = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20)).Frozen();
		static readonly Pen s_bodyPen = new Pen(new SolidColorBrush(Colors.White).Frozen(), 1.0).Frozen();
		static readonly Pen s_pathPen = new Pen(new SolidColorBrush(Colors.Gray).Frozen(), 1.0).Frozen();
		Point m_mouseDownPosition;
		bool m_isDragging;
		Point m_mouseDownCenter;
		bool m_isMouseDown;
	}
}
