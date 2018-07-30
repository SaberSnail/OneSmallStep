﻿using System.Linq;
using System.Windows;
using System.Windows.Media;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.ECS;
using OneSmallStep.ECS.Components;
using OneSmallStep.SystemMap;

namespace OneSmallStep.EntityViewModels
{
	public sealed class ShipViewModel : EntityViewModelBase, ISystemBodyRenderer
	{
		public ShipViewModel(Entity entity)
			: base(entity)
		{
		}

		public string PositionString
		{
			get
			{
				VerifyAccess();
				return m_positionString;
			}
			private set
			{
				SetPropertyField(nameof(PositionString), value, ref m_positionString);
			}
		}

		public Point Position
		{
			get
			{
				VerifyAccess();
				return m_position;
			}
			private set
			{
				SetPropertyField(nameof(Position), value, ref m_position);
			}
		}

		public Point? TargetPosition
		{
			get
			{
				VerifyAccess();
				return m_targetPosition;
			}
			private set
			{
				SetPropertyField(nameof(TargetPosition), value, ref m_targetPosition);
			}
		}

		public override void UpdateFromEntity()
		{
			var body = Entity.GetRequiredComponent<OrbitalPositionComponent>();
			Position = body.GetCurrentAbsolutePosition();
			PositionString = "{0}, {1}".FormatCurrentCulture(Position.X, Position.Y);

			var order = Entity.GetRequiredComponent<MovementOrdersComponent>().Orders.FirstOrDefault() as MoveToOrbitalBodyOrder;
			TargetPosition = order?.InterceptPoint ?? new Point();
		}

		public void Render(DrawingContext context, Point offset, double scale)
		{
			var renderAt = new Point((Position.X * scale) + offset.X, (Position.Y * scale) + offset.Y);
			context.DrawLine(s_markerPen, new Point(renderAt.X - 4, renderAt.Y), new Point(renderAt.X + 4, renderAt.Y));
			context.DrawLine(s_markerPen, new Point(renderAt.X, renderAt.Y - 4), new Point(renderAt.X, renderAt.Y + 4));
			if (TargetPosition != null)
			{
				var renderTo = new Point((TargetPosition.Value.X * scale) + offset.X, (TargetPosition.Value.Y * scale) + offset.Y);
				context.DrawLine(s_pathPen, renderAt, renderTo);
			}
		}

		static readonly Pen s_markerPen = new Pen(new SolidColorBrush(Colors.White).Frozen(), 1.0).Frozen();
		static readonly Pen s_pathPen = new Pen(new SolidColorBrush(Colors.Gray).Frozen(), 1.0).Frozen();

		string m_positionString;
		Point m_position;
		Point? m_targetPosition;
	}
}