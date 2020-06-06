using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.ECS.Components;

namespace OneSmallStep.UI.Controls
{
	public sealed class CohortsControl : Canvas
	{
		public static readonly DependencyProperty CohortsProperty = DependencyPropertyUtility<CohortsControl>.Register(x => x.Cohorts, null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure);

		public CohortCollection Cohorts
		{
			get => (CohortCollection) GetValue(CohortsProperty);
			set => SetValue(CohortsProperty, value);
		}

		public static readonly DependencyProperty PaddingProperty = DependencyPropertyUtility<CohortsControl>.Register(x => x.Padding, null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure);

		public Thickness Padding
		{
			get => (Thickness) GetValue(PaddingProperty);
			set => SetValue(PaddingProperty, value);
		}

		public static readonly DependencyProperty OrientationProperty = DependencyPropertyUtility<CohortsControl>.Register(x => x.Orientation, null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure);

		public Orientation Orientation
		{
			get => (Orientation) GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		public static readonly DependencyProperty ItemPaddingProperty = DependencyPropertyUtility<CohortsControl>.Register(x => x.ItemPadding, null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, 3.0);

		public double ItemPadding
		{
			get => (double) GetValue(ItemPaddingProperty);
			set => SetValue(ItemPaddingProperty, value);
		}

		public static readonly DependencyProperty MinItemSizeProperty = DependencyPropertyUtility<CohortsControl>.Register(x => x.MinItemSize, null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, 5.0);

		public double MinItemSize
		{
			get => (double) GetValue(MinItemSizeProperty);
			set => SetValue(MinItemSizeProperty, value);
		}

		public static readonly DependencyProperty ItemPenProperty = DependencyPropertyUtility<CohortsControl>.Register(x => x.ItemPen, null, FrameworkPropertyMetadataOptions.AffectsRender);

		public Pen ItemPen
		{
			get => (Pen) GetValue(ItemPenProperty);
			set => SetValue(ItemPenProperty, value);
		}

		public static readonly DependencyProperty ItemBrushProperty = DependencyPropertyUtility<CohortsControl>.Register(x => x.ItemBrush, null, FrameworkPropertyMetadataOptions.AffectsRender);

		public Brush ItemBrush
		{
			get => (Brush) GetValue(ItemBrushProperty);
			set => SetValue(ItemBrushProperty, value);
		}

		public static readonly DependencyProperty CenterItemsProperty = DependencyPropertyUtility<CohortsControl>.Register(x => x.CenterItems, null, true);

		public bool CenterItems
		{
			get => (bool) GetValue(CenterItemsProperty);
			set => SetValue(CenterItemsProperty, value);
		}

		protected override Size MeasureOverride(Size constraint)
		{
			var itemCount = Cohorts?.Cohorts.Count ?? 0;
			var padding = Padding;
			double minWidth;
			double minHeight;
			if (Orientation == Orientation.Vertical)
			{
				minWidth = padding.Left + padding.Right;
				minHeight = itemCount * MinItemSize + Math.Min(itemCount - 1, 0) * ItemPadding + padding.Top + padding.Bottom;
			}
			else
			{
				minWidth = itemCount * MinItemSize + Math.Min(itemCount - 1, 0) * ItemPadding + padding.Left + padding.Right;
				minHeight = padding.Top + padding.Bottom;
			}
			return new Size(Math.Min(constraint.Width, minWidth), Math.Min(constraint.Height, minHeight));
		}

		protected override void OnRender(DrawingContext context)
		{
			base.OnRender(context);

			var viewRect = new Rect(0, 0, ActualWidth, ActualHeight);
			context.DrawRectangle(Background, null, viewRect);

			var cohorts = Cohorts;
			if (cohorts is object && cohorts.Cohorts.Count != 0)
			{
				var maxPopulation = cohorts.Cohorts.Select(x => x.Population).Max();
				var margin = Padding;
				var padding = ItemPadding;
				var itemBrush = ItemBrush;
				var itemPen = ItemPen;

				if (Orientation == Orientation.Vertical)
				{
					var itemLeft = viewRect.Left + margin.Left;
					var maxItemWidth = viewRect.Width - margin.Left - margin.Right;
					var availableHeight = viewRect.Height - margin.Top - margin.Bottom - (padding * (cohorts.Cohorts.Count - 1));
					if (availableHeight > 0)
					{
						var itemHeight = availableHeight / cohorts.Cohorts.Count;
						var top = viewRect.Top + margin.Top;
						var centerItems = CenterItems;
						foreach (var cohort in cohorts.Cohorts.Reverse())
						{
							var itemWidth = maxItemWidth * cohort.Population / maxPopulation;
							var left = centerItems ? itemLeft + ((maxItemWidth - itemWidth) / 2.0) : itemLeft;
							var itemRect = new Rect(left, top, itemWidth, itemHeight);
							context.DrawRectangle(itemBrush, itemPen, itemRect);

							top += itemRect.Height + padding;
						}
					}
				}
				else
				{
					var itemTop = viewRect.Top + margin.Top;
					var maxItemHeight = viewRect.Height - margin.Top - margin.Bottom;
					var availableWidth = viewRect.Width - margin.Left - margin.Right - (padding * (cohorts.Cohorts.Count - 1));
					if (availableWidth > 0)
					{
						var itemWidth = availableWidth / cohorts.Cohorts.Count;
						var left = viewRect.Left + margin.Left;
						var centerItems = CenterItems;
						foreach (var cohort in cohorts.Cohorts)
						{
							var itemHeight = maxItemHeight * cohort.Population / maxPopulation;
							var top = centerItems ? itemTop + ((maxItemHeight - itemHeight) / 2.0) : itemTop + maxItemHeight - itemHeight;
							var itemRect = new Rect(left, top, itemWidth, itemHeight);
							context.DrawRectangle(itemBrush, itemPen, itemRect);

							left += itemRect.Width + padding;
						}
					}
				}
			}
		}
	}
}
