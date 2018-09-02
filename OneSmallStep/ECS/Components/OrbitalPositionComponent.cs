using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.Time;
using OneSmallStep.Utility;

namespace OneSmallStep.ECS.Components
{
	public sealed class OrbitalPositionComponent : ComponentBase
	{
		public static OrbitalPositionComponent CreatePoweredBody(Point startingPoint)
		{
			return new OrbitalPositionComponent(null, startingPoint, null, null);
		}

		public static OrbitalPositionComponent CreateUnpoweredBody()
		{
			return new OrbitalPositionComponent(null, new Point(), null, null);
		}

		public static OrbitalPositionComponent CreateUnpoweredBody(Entity parent, double mass, double periodInDays, double meanAnomalyInDegrees, bool isPrograde, ICalendar calendar)
		{
			if (parent == null)
				throw new ArgumentNullException(nameof(parent));

			var parentMass = parent.GetRequiredComponent<OrbitalBodyCharacteristicsComponent>().Mass;
			var mu = Constants.GravitationalConstant * (mass + parentMass);
			var angularVelocity = (isPrograde ? 1 : -1) * 2.0 * Math.PI / (periodInDays * 24.0 * 3600.0);
			var orbitalRadius = Math.Pow(mu / (angularVelocity * angularVelocity), 1.0 / 3.0);

			var secondsFromEpoch = calendar.CreateTimePoint(2000, 1, 1).Tick * Constants.SecondsPerTick;
			var initialAngle = (secondsFromEpoch * angularVelocity) + meanAnomalyInDegrees / (2.0 * Math.PI);
			var relativePosition = new Point(orbitalRadius * Math.Cos(initialAngle), orbitalRadius * Math.Sin(initialAngle));
			var angularVelocityPerTick = angularVelocity * Constants.SecondsPerTick;

			return new OrbitalPositionComponent(parent.Id, relativePosition, angularVelocityPerTick, orbitalRadius);
		}

		public EntityId? ParentId
		{
			get => m_parentId;
			set => SetPropertyField(value, ref m_parentId);
		}

		public Point RelativePosition
		{
			get => m_relativePosition;
			set => SetPropertyField(value, ref m_relativePosition);
		}

		public double? AngularVelocityPerTick
		{
			get => m_angularVelocityPerTick;
			set => SetPropertyField(value, ref m_angularVelocityPerTick);
		}

		public double? OrbitalRadius
		{
			get => m_orbitalRadius;
			set => SetPropertyField(value, ref m_orbitalRadius);
		}

		public Point GetCurrentAbsolutePosition(IEntityLookup entityLookup)
		{
			var position = m_relativePosition;
			var parentAbsolutePosition = GetParentEntity(entityLookup)?.GetOptionalComponent<OrbitalPositionComponent>()?.GetCurrentAbsolutePosition(entityLookup);

			if (parentAbsolutePosition != null)
				position = position.WithOffset(parentAbsolutePosition.Value);
			return position;
		}

		public void SetCurrentAbsolutePosition(Point absolutePosition)
		{
			using (ScopedPropertyChange())
			{
				m_angularVelocityPerTick = null;
				m_orbitalRadius = null;
				m_relativePosition = absolutePosition;
			}
		}

		public void InitializeOrbitalValuesIfNeeded(IEntityLookup entityLookup, double mass)
		{
			if (m_angularVelocityPerTick.HasValue)
				return;

			using (ScopedPropertyChange())
			{
				m_orbitalRadius = m_relativePosition.DistanceTo(new Point());

				var parent = GetParentEntity(entityLookup);
				var parentPosition = parent?.GetOptionalComponent<OrbitalPositionComponent>();
				if (parentPosition != null)
				{
					var parentBody = parent.GetRequiredComponent<OrbitalBodyCharacteristicsComponent>();
					var mu = Constants.GravitationalConstant * (mass + parentBody.Mass);
					m_angularVelocityPerTick = Math.Sqrt(mu / (m_orbitalRadius.Value * m_orbitalRadius.Value * m_orbitalRadius.Value)) * Constants.SecondsPerTick;
				}
				else
				{
					m_angularVelocityPerTick = 0.0;
				}
			}
		}

		public Point GetRelativeOrbitalPositionAtTime(TimeOffset timeOffset)
		{
			var currentAngle = Math.Atan2(m_relativePosition.Y, m_relativePosition.X);
			var angle = currentAngle + (m_angularVelocityPerTick.Value * timeOffset.TickOffset);
			return new Point(Math.Cos(angle) * m_orbitalRadius.Value, Math.Sin(angle) * m_orbitalRadius.Value);
		}

		public Point GetAbsoluteOrbitalPositionAtTime(IEntityLookup entityLookup, TimeOffset timeOffset)
		{
			var parentPosition = GetParentEntity(entityLookup)?.GetOptionalComponent<OrbitalPositionComponent>()?.GetAbsoluteOrbitalPositionAtTime(entityLookup, timeOffset);
			if (parentPosition == null)
				return m_relativePosition;
			return GetRelativeOrbitalPositionAtTime(timeOffset).WithOffset(parentPosition.Value);
		}

		public IEnumerable<OrbitalPositionComponent> EnumerateThisAndParents(IEntityLookup entityLookup)
		{
			var parent = GetParentEntity(entityLookup)?.GetOptionalComponent<OrbitalPositionComponent>();
			if (parent == null)
				return Enumerable.Repeat(this, 1);
			return parent.EnumerateThisAndParents(entityLookup).Prepend(this);
		}

		public override ComponentBase Clone()
		{
			return new OrbitalPositionComponent(this);
		}

		private OrbitalPositionComponent(EntityId? parentId, Point relativePosition, double? angularVelocityPerTick, double? orbitalRadius)
		{
			m_parentId = parentId;
			m_relativePosition = relativePosition;
			m_angularVelocityPerTick = angularVelocityPerTick;
			m_orbitalRadius = orbitalRadius;
		}

		private OrbitalPositionComponent(OrbitalPositionComponent that)
		: base(that)
		{
			m_parentId = that.m_parentId;
			m_relativePosition = that.m_relativePosition;
			m_angularVelocityPerTick = that.m_angularVelocityPerTick;
			m_orbitalRadius = that.m_orbitalRadius;
		}

		private Entity GetParentEntity(IEntityLookup entityLookup)
		{
			return m_parentId.HasValue ? entityLookup.GetEntity(m_parentId.Value) : null;
		}

		EntityId? m_parentId;
		Point m_relativePosition;
		double? m_angularVelocityPerTick;
		double? m_orbitalRadius;
	}
}
