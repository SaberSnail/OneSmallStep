using System.Windows;
using System.Windows.Media;
using GoldenAnvil.Utility;
using OneSmallStep.ECS;
using OneSmallStep.ECS.Components;
using OneSmallStep.UI.SystemMap;
using OneSmallStep.UI.Utility;

namespace OneSmallStep.UI.EntityViewModels
{
	public sealed class ShipViewModel : EntityViewModelBase, ISystemBodyRenderer
	{
		public ShipViewModel(Entity entity)
			: base(entity.Id)
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
				SetPropertyField(value, ref m_positionString);
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
				SetPropertyField(value, ref m_position);
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
				SetPropertyField(value, ref m_targetPosition);
			}
		}

		public Point? TargetCurrentPosition
		{
			get
			{
				VerifyAccess();
				return m_targetCurrentPosition;
			}
			private set
			{
				SetPropertyField(value, ref m_targetCurrentPosition);
			}
		}

		public override void UpdateFromEntity(IEntityLookup entityLookup)
		{
			var entity = entityLookup.GetEntity(EntityId);

			var body = entity.GetRequiredComponent<OrbitalPositionComponent>();
			Position = body.GetCurrentAbsolutePosition(entityLookup);
			PositionString = "{0}, {1}".FormatCurrentCulture(Position.X, Position.Y);

			if (entity.GetRequiredComponent<MovementOrdersComponent>().GetActiveOrder() is MoveToOrbitalBodyOrder order)
			{
				TargetPosition = order.InterceptPoint;
				var targetEntity = entityLookup.GetEntity(order.TargetEntityId);
				var targetBody = targetEntity.GetRequiredComponent<OrbitalPositionComponent>();
				TargetCurrentPosition = targetBody.GetCurrentAbsolutePosition(entityLookup);
			}
			else
			{
				TargetPosition = null;
				TargetCurrentPosition = null;
			}
		}

		public void Render(DrawingContext context, Point offset, double scale)
		{
			var renderAt = new Point((Position.X * scale) + offset.X, (Position.Y * scale) + offset.Y);
			var markerPen = (Pen) ThemesUtility.CurrentThemeDictionary["ShipMarkerPen"];
			context.DrawLine(markerPen, new Point(renderAt.X - 4, renderAt.Y), new Point(renderAt.X + 4, renderAt.Y));
			context.DrawLine(markerPen, new Point(renderAt.X, renderAt.Y - 4), new Point(renderAt.X, renderAt.Y + 4));

			if (TargetPosition != null)
			{
				var renderTo = new Point((TargetPosition.Value.X * scale) + offset.X, (TargetPosition.Value.Y * scale) + offset.Y);
				var pathPen = (Pen) ThemesUtility.CurrentThemeDictionary["ShipPathPen"];
				context.DrawLine(pathPen, renderAt, renderTo);
			}

			if (TargetCurrentPosition != null)
			{
				var renderTo = new Point((TargetCurrentPosition.Value.X * scale) + offset.X, (TargetCurrentPosition.Value.Y * scale) + offset.Y);
				var targetPathPen = (Pen) ThemesUtility.CurrentThemeDictionary["ShipTargetPathPen"];
				context.DrawLine(targetPathPen, renderAt, renderTo);
			}
		}

		string m_positionString;
		Point m_position;
		Point? m_targetPosition;
		Point? m_targetCurrentPosition;
	}
}
