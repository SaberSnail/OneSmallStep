using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.ECS;
using OneSmallStep.ECS.Components;
using OneSmallStep.Time;

namespace OneSmallStep.SystemMap
{
	public class SystemMapControl : Canvas
	{
		public static readonly DependencyProperty DateProperty = DependencyPropertyUtility<SystemMapControl>.Register(x => x.Date, OnDateChanged);

		public string Date
		{
			get { return (string) GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}

		public static readonly DependencyProperty EntitiesProperty = DependencyPropertyUtility<SystemMapControl>.Register(x => x.Entities, OnEntitiesChanged);

		public IReadOnlyList<Entity> Entities
		{
			get { return (IReadOnlyList<Entity>) GetValue(EntitiesProperty); }
			set { SetValue(EntitiesProperty, value); }
		}

		protected override void OnRender(DrawingContext context)
		{
			base.OnRender(context);

			var actualRect = new Rect(0, 0, ActualWidth, ActualHeight);
			context.DrawRectangle(s_background, null, actualRect);

			var centerPoint = new Point(actualRect.Width / 2.0, actualRect.Height / 2.0);
			var scale = Math.Min(centerPoint.X, centerPoint.Y) / 2.5E11;
			foreach (var entity in Entities.EmptyIfNull())
			{
				var body = entity.GetComponent<AstronomicalBodyComponent>();
				var position = body.GetAbsolutePosition();
				var renderAt = new Point(centerPoint.X + (position.X * scale), centerPoint.Y + (position.Y * scale));
				context.DrawEllipse(s_bodyBrush, s_bodyPen, renderAt, 4, 4);
			}
		}

		private static void OnEntitiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((SystemMapControl) d).InvalidateVisual();
		}

		private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((SystemMapControl) d).InvalidateVisual();
		}

		static readonly Brush s_background = new SolidColorBrush(Colors.Black).Frozen();
		static readonly Brush s_bodyBrush = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20)).Frozen();
		static readonly Pen s_bodyPen = new Pen(new SolidColorBrush(Colors.White).Frozen(), 1.0).Frozen();
	}
}
