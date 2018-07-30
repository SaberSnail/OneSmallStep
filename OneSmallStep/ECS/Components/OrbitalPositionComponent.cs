using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GoldenAnvil.Utility.Windows;
using JetBrains.Annotations;
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

		public static OrbitalPositionComponent CreateUnpoweredBody([NotNull] Entity parent, double mass, double periodInDays, double meanAnomalyInDegrees, bool isPrograde, ICalendar calendar)
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

			return new OrbitalPositionComponent(parent, relativePosition, angularVelocityPerTick, orbitalRadius);
		}

		public Entity Parent { get; set; }
		public Point RelativePosition { get; set; }
		public double? AngularVelocityPerTick { get; set; }
		public double? OrbitalRadius { get; set; }

		public Point GetCurrentAbsolutePosition()
		{
			var position = RelativePosition;
			var parentAbsolutePosition = Parent?.GetOptionalComponent<OrbitalPositionComponent>()?.GetCurrentAbsolutePosition();
			if (parentAbsolutePosition != null)
				position = position.WithOffset(parentAbsolutePosition.Value);
			return position;
		}

		public void SetCurrentAbsolutePosition(Point absolutePosition)
		{
			AngularVelocityPerTick = null;
			OrbitalRadius = null;
			RelativePosition = absolutePosition;
		}

		public void InitializeOrbitalValuesIfNeeded(double mass)
		{
			if (AngularVelocityPerTick.HasValue)
				return;

			OrbitalRadius = RelativePosition.DistanceTo(new Point());

			var parentPosition = Parent?.GetOptionalComponent<OrbitalPositionComponent>();
			if (parentPosition != null)
			{
				var parentBody = Parent.GetRequiredComponent<OrbitalBodyCharacteristicsComponent>();
				var mu = Constants.GravitationalConstant * (mass + parentBody.Mass);
				AngularVelocityPerTick = Math.Sqrt(mu / (OrbitalRadius.Value * OrbitalRadius.Value * OrbitalRadius.Value)) * Constants.SecondsPerTick;
			}
			else
			{
				AngularVelocityPerTick = 0;
			}
		}

		public Point GetRelativeOrbitalPositionAtTime(TimeOffset timeOffset, double mass)
		{
			var currentAngle = Math.Atan2(RelativePosition.Y, RelativePosition.X);
			var angle = currentAngle + (AngularVelocityPerTick.Value * timeOffset.TickOffset);
			return new Point(Math.Cos(angle) * OrbitalRadius.Value, Math.Sin(angle) * OrbitalRadius.Value);
		}

		public Point GetAbsoluteOrbitalPositionAtTime(TimeOffset timeOffset, double mass)
		{
			var parentMass = Parent?.GetOptionalComponent<OrbitalBodyCharacteristicsComponent>()?.Mass ?? 0.0;
			var parentPosition = Parent?.GetOptionalComponent<OrbitalPositionComponent>()?.GetAbsoluteOrbitalPositionAtTime(timeOffset, parentMass);
			if (parentPosition == null)
				return RelativePosition;
			return GetRelativeOrbitalPositionAtTime(timeOffset, mass).WithOffset(parentPosition.Value);
		}

		public IEnumerable<OrbitalPositionComponent> EnumerateThisAndParents()
		{
			var parent = Parent?.GetOptionalComponent<OrbitalPositionComponent>();
			if (parent == null)
				return Enumerable.Repeat(this, 1);
			return parent.EnumerateThisAndParents().Prepend(this);
		}

		private OrbitalPositionComponent(Entity parent, Point relativePosition, double? angularVelocityPerTick, double? orbitalRadius)
		{
			Parent = parent;
			RelativePosition = relativePosition;
			AngularVelocityPerTick = angularVelocityPerTick;
			OrbitalRadius = orbitalRadius;
		}
	}
}
