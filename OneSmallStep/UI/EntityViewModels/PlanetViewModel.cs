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

		public string Name
		{
			get
			{
				VerifyAccess();
				return m_name;
			}
			private set
			{
				SetPropertyField(value, ref m_name);
			}
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

		public double SemiMajorAxis
		{
			get
			{
				VerifyAccess();
				return m_semiMajorAxis;
			}
			private set
			{
				SetPropertyField(value, ref m_semiMajorAxis);
			}
		}

		public double SemiMinorAxis
		{
			get
			{
				VerifyAccess();
				return m_semiMinorAxis;
			}
			private set
			{
				SetPropertyField(value, ref m_semiMinorAxis);
			}
		}

		public double Focus
		{
			get
			{
				VerifyAccess();
				return m_focus;
			}
			private set
			{
				SetPropertyField(value, ref m_focus);
			}
		}

		public double LongitudeOfPeriapsis
		{
			get
			{
				VerifyAccess();
				return m_longitudeOfPeriapsis;
			}
			private set
			{
				SetPropertyField(value, ref m_longitudeOfPeriapsis);
			}
		}

		public override void UpdateFromEntity(IEntityLookup entityLookup)
		{
			var entity = entityLookup.GetEntity(EntityId);

			var information = entity.GetOptionalComponent<InformationComponent>();
			Name = information?.Name;

			var population = entity.GetOptionalComponent<PopulationComponent>();
			Population = population?.Population ?? 0;

			var position = entity.GetRequiredComponent<EllipticalOrbitalPositionComponent>();
			var body = entity.GetRequiredComponent<OrbitalBodyCharacteristicsComponent>();
			Radius = body.Radius;
			var parentEntity = position.ParentId.HasValue ? entityLookup.GetEntity(position.ParentId.Value) : null;
			OrbitCenterPosition = parentEntity?.GetOptionalComponent<EllipticalOrbitalPositionComponent>()?.GetCurrentAbsolutePosition(entityLookup) ?? new Point();
			Position = position.RelativePosition.WithOffset(OrbitCenterPosition);
			PositionString = "{0}, {1}".FormatCurrentCulture(Position.X, Position.Y);
			SemiMajorAxis = position.SemiMajorAxis ?? 0.0;
			SemiMinorAxis = position.SemiMinorAxis ?? 0.0;
			Focus = position.Focus ?? 0.0;
			LongitudeOfPeriapsis = position.LongitudeOfPeriapsis ?? 0.0;
		}

		public void Render(DrawingContext context, Point offset, double scale)
		{
			if (m_position != m_orbitCenterPosition && !Name.StartsWith("Asteroid"))
			{
				var focus = Focus * scale;
				var renderOrbitAt = new Point((OrbitCenterPosition.X * scale) - focus + offset.X, (OrbitCenterPosition.Y * scale) + offset.Y);
				var semiMajorAxis = SemiMajorAxis * scale;
				var semiMinorAxis = SemiMinorAxis * scale;
				var orbitPen = (Pen) ThemesUtility.CurrentThemeDictionary["PlanetOrbitPen"];
				using (context.ScopedTransform(new TranslateTransform(renderOrbitAt.X, renderOrbitAt.Y)))
				using (context.ScopedTransform(new RotateTransform(LongitudeOfPeriapsis, focus, 0.0)))
					context.DrawEllipse(null, orbitPen, new Point(), semiMajorAxis, semiMinorAxis);
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
				var lightColor = ((SolidColorBrush) ThemesUtility.CurrentThemeDictionary["PlanetBodyLightBrush"]).Color;
				var darkColor = ((SolidColorBrush) ThemesUtility.CurrentThemeDictionary["PlanetBodyDarkBrush"]).Color;
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
		double m_semiMajorAxis;
		double m_semiMinorAxis;
		double m_radius;
		double m_focus;
		double m_longitudeOfPeriapsis;
		string m_name;
	}
}
