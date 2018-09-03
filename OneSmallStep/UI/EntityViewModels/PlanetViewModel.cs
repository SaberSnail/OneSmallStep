using System;
using System.Windows;
using System.Windows.Media;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.ECS;
using OneSmallStep.ECS.Components;
using OneSmallStep.UI.SystemMap;
using OneSmallStep.UI.Utility;

namespace OneSmallStep.UI.EntityViewModels
{
	public sealed class PlanetViewModel : EntityViewModelBase, ISystemBodyRenderer
	{
		public PlanetViewModel(Entity entity)
			: base(entity.Id)
		{
		}

		public long Population
		{
			get
			{
				VerifyAccess();
				return m_population;
			}
			private set
			{
				SetPropertyField(value, ref m_population);
			}
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

		public double Radius
		{
			get
			{
				VerifyAccess();
				return m_radius;
			}
			private set
			{
				SetPropertyField(value, ref m_radius);
			}
		}

		public Point OrbitCenterPosition
		{
			get
			{
				VerifyAccess();
				return m_orbitCenterPosition;
			}
			private set
			{
				SetPropertyField(value, ref m_orbitCenterPosition);
			}
		}

		public double OrbitalRadius
		{
			get
			{
				VerifyAccess();
				return m_orbitalRadius;
			}
			private set
			{
				SetPropertyField(value, ref m_orbitalRadius);
			}
		}

		public override void UpdateFromEntity(IEntityLookup entityLookup)
		{
			var entity = entityLookup.GetEntity(EntityId);

			var population = entity.GetOptionalComponent<PopulationComponent>();
			Population = population?.Population ?? 0;

			var position = entity.GetRequiredComponent<OrbitalPositionComponent>();
			var body = entity.GetRequiredComponent<OrbitalBodyCharacteristicsComponent>();
			Radius = body.Radius;
			PositionString = "{0}, {1}".FormatCurrentCulture(Position.X, Position.Y);
			var parentEntity = position.ParentId.HasValue ? entityLookup.GetEntity(position.ParentId.Value) : null;
			OrbitCenterPosition = parentEntity?.GetOptionalComponent<OrbitalPositionComponent>()?.GetCurrentAbsolutePosition(entityLookup) ?? new Point();
			Position = position.RelativePosition.WithOffset(OrbitCenterPosition);
			OrbitalRadius = position.OrbitalRadius ?? 0.0;
		}

		public void Render(DrawingContext context, Point offset, double scale)
		{
			if (m_position != m_orbitCenterPosition)
			{
				var renderOrbitAt = new Point((OrbitCenterPosition.X * scale) + offset.X, (OrbitCenterPosition.Y * scale) + offset.Y);
				var orbitRadius = OrbitalRadius * scale;
				var orbitPen = (Pen) ThemesUtility.CurrentThemeDictionary["PlanetOrbitPen"];
				context.DrawEllipse(null, orbitPen, renderOrbitAt, orbitRadius, orbitRadius);
			}

			var minRadius = (double) ThemesUtility.CurrentThemeDictionary["PlanetMinRadius"];
			var radius = Math.Max(minRadius, Radius * scale);
			var renderAt = new Point((Position.X * scale) + offset.X, (Position.Y * scale) + offset.Y);
			var bodyBrush = (Brush) ThemesUtility.CurrentThemeDictionary["PlanetBodyBrush"];
			if (Position != new Point())
			{
				var vectorToCenter = renderAt.VectorTo(offset);
				vectorToCenter.Normalize();
				var gradientStart = renderAt + (vectorToCenter * 10.0);
				var lightColor = (Color) ThemesUtility.CurrentThemeDictionary["PlanetBodyLightColor"];
				var darkColor = (Color) ThemesUtility.CurrentThemeDictionary["PlanetBodyDarkColor"];
				var gradientBrush = new LinearGradientBrush(new GradientStopCollection
					{
						new GradientStop(lightColor, 0.0),
						new GradientStop(lightColor, 1.0),
						new GradientStop(darkColor, 1.0),
					},
					gradientStart,
					renderAt
				);
				gradientBrush.MappingMode = BrushMappingMode.Absolute;
				gradientBrush.SpreadMethod = GradientSpreadMethod.Pad;
				bodyBrush = gradientBrush.Frozen();
			}

			var bodyPen = (Pen) ThemesUtility.CurrentThemeDictionary["PlanetBodyPen"];
			context.DrawEllipse(bodyBrush, bodyPen, renderAt, radius, radius);
		}

		long m_population;
		string m_positionString;
		Point m_position;
		Point m_orbitCenterPosition;
		double m_orbitalRadius;
		double m_radius;
	}
}
