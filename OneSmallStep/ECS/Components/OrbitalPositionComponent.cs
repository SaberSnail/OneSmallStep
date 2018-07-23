using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using OneSmallStep.Time;
using OneSmallStep.Utility;

namespace OneSmallStep.ECS.Components
{
	public sealed class OrbitalPositionComponent : ComponentBase
	{
		public static OrbitalPositionComponent CreatePoweredBody(double maxSpeed, double mass, double radius, Point startingPoint)
		{
			var data = new PoweredStarSystemPositionData
			{
				MaxSpeed = maxSpeed,
			};
			return new OrbitalPositionComponent(data)
			{
				Mass = mass,
				Radius = radius,
				RelativePosition = startingPoint,
			};
		}

		public static OrbitalPositionComponent CreateUnpoweredBody(double mass, double radius)
		{
			return new OrbitalPositionComponent(new UnpoweredStarSystemPositionData())
			{
				Mass = mass,
				Radius = radius,
				RelativePosition = new Point(0, 0),
			};
		}

		public static OrbitalPositionComponent CreateUnpoweredBody(Entity parentEntity, double mass, double radius, double periodInDays, double meanAnomalyInDegrees, bool isPrograde, ICalendar calendar)
		{
			var parentBody = parentEntity.GetComponent<OrbitalPositionComponent>();
			var mu = Constants.GravitationalConstant * (mass + parentBody.Mass);
			var angularVelocity = (isPrograde ? 1 : -1) * 2.0 * Math.PI / (periodInDays * 24.0 * 3600.0);
			var orbitalRadius = Math.Pow(mu / (angularVelocity * angularVelocity), 1.0 / 3.0);

			var secondsFromEpoch = calendar.CreateTimePoint(2000, 1, 1).Tick * Constants.SecondsPerTick;
			var initialAngle = (secondsFromEpoch * angularVelocity) + meanAnomalyInDegrees / (2.0 * Math.PI);
			var relativePosition = new Point(orbitalRadius * Math.Cos(initialAngle), orbitalRadius * Math.Sin(initialAngle));

			var data = new UnpoweredStarSystemPositionData
			{
				AngularVelocity = angularVelocity,
				Mu = mu,
				OrbitalRadius = orbitalRadius,
			};

			return new OrbitalPositionComponent(data)
			{
				Mass = mass,
				Radius = radius,
				RelativePosition = relativePosition,
				Parent = parentEntity,
			};
		}

		public Entity Parent { get; set; }
		public Point RelativePosition { get; set; }
		public double Mass { get; set; }
		public double Radius { get; set; }

		public Point GetAbsolutePosition() => m_positionData.GetAbsolutePosition(this);

		public Point GetAbsolutePositionAtTime(double ticks) => m_positionData.GetAbsolutePositionAtTime(this, ticks);

		public Point? GetInterceptPoint(Point interceptorPosition, double interceptorMaxSpeed) => m_positionData.GetInterceptPoint(this, interceptorPosition, interceptorMaxSpeed);

		public void EnsureStartValidity() => m_positionData.EnsureStartValidity(this);

		public void EnsureEndValidity() => m_positionData.EnsureEndValidity(this);

		public void MoveOneTick() => m_positionData.MoveOneTick(this);

		public void TrySetTarget(Entity target) => m_positionData.TrySetTarget(target);

		public Point? TryGetTargetPoint() => m_positionData.TryGetTargetPoint();

		public IEnumerable<OrbitalPositionComponent> EnumerateThisAndParents()
		{
			var parent = Parent?.GetComponent<OrbitalPositionComponent>();
			if (parent == null)
				return Enumerable.Repeat(this, 1);
			return parent.EnumerateThisAndParents().Prepend(this);
		}

		private OrbitalPositionComponent(StarSystemPositionDataBase positionData) => m_positionData = positionData;

		StarSystemPositionDataBase m_positionData;
	}
}
