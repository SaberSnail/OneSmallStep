using System;
using System.Windows;
using System.Windows.Media;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.ECS;
using OneSmallStep.ECS.Components;
using OneSmallStep.SystemMap;

namespace OneSmallStep.EntityViewModels
{
	public sealed class PlanetViewModel : EntityViewModelBase, ISystemBodyRenderer
	{
		public PlanetViewModel(Entity entity)
			: base(entity)
		{
		}

		public long Population
		{
			get
			{
				VerifyAccess();
				return m_population;
			}
			set
			{
				if (SetPropertyField(nameof(Population), value, ref m_population))
				{
					var populationComponent = Entity.GetComponent<PopulationComponent>();
					if (populationComponent != null)
						populationComponent.Population = m_population;
				}
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

		public double Radius
		{
			get
			{
				VerifyAccess();
				return m_radius;
			}
			set
			{
				SetPropertyField(nameof(Radius), value, ref m_radius);
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
				SetPropertyField(nameof(OrbitCenterPosition), value, ref m_orbitCenterPosition);
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
				SetPropertyField(nameof(OrbitalRadius), value, ref m_orbitalRadius);
			}
		}

		public override void UpdateFromEntity()
		{
			var population = Entity.GetComponent<PopulationComponent>();
			Population = population?.Population ?? 0;

			var body = Entity.GetComponent<OrbitalPositionComponent>();
			Position = body?.GetAbsolutePosition() ?? new Point();
			Radius = body?.Radius ?? 0.0;
			PositionString = "{0}, {1}".FormatCurrentCulture(Position.X, Position.Y);
			OrbitCenterPosition = body?.Parent?.GetComponent<OrbitalPositionComponent>()?.GetAbsolutePosition() ?? new Point();
			OrbitalRadius = Position.DistanceTo(OrbitCenterPosition);
		}

		public void Render(DrawingContext context, Point offset, double scale)
		{
			if (m_position != m_orbitCenterPosition)
			{
				var renderOrbitAt = new Point((OrbitCenterPosition.X * scale) + offset.X, (OrbitCenterPosition.Y * scale) + offset.Y);
				var orbitRadius = OrbitalRadius * scale;
				context.DrawEllipse(null, s_orbitPen, renderOrbitAt, orbitRadius, orbitRadius);
			}

			var radius = Math.Max(c_minRadius, Radius * scale);
			var renderAt = new Point((Position.X * scale) + offset.X, (Position.Y * scale) + offset.Y);
			context.DrawEllipse(s_bodyBrush, s_bodyPen, renderAt, radius, radius);
		}

		static readonly Brush s_bodyBrush = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20)).Frozen();
		static readonly Pen s_bodyPen = new Pen(new SolidColorBrush(Colors.White).Frozen(), 1.0).Frozen();
		static readonly Pen s_orbitPen = new Pen(new SolidColorBrush(Color.FromRgb(0x30, 0x30, 0x30)), 1.0).Frozen();
		const double c_minRadius = 4.0;

		long m_population;
		string m_positionString;
		Point m_position;
		Point m_orbitCenterPosition;
		double m_orbitalRadius;
		double m_radius;
	}
}
