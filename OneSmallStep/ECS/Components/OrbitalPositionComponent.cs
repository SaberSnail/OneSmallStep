using System;
using System.Windows;
using OneSmallStep.Time;
using OneSmallStep.Utility;

namespace OneSmallStep.ECS.Components
{
	public sealed class OrbitalPositionComponent : ComponentBase
	{
		public static OrbitalPositionComponent CreatePoweredBody(double maxSpeed, double mass, Point startingPoint, Entity target)
		{
			var data = new PoweredStarSystemPositionData
			{
				MaxSpeed = maxSpeed,
				TargetEntity = target,
			};
			return new OrbitalPositionComponent(data)
			{
				Mass = mass,
				RelativePosition = startingPoint,
			};
		}

		public static OrbitalPositionComponent CreateUnpoweredBody(double mass)
		{
			return new OrbitalPositionComponent(new UnpoweredStarSystemPositionData())
			{
				Mass = mass,
				RelativePosition = new Point(0, 0),
			};
		}

		public static OrbitalPositionComponent CreateUnpoweredBody(Entity parentEntity, double mass, double periodInDays, double meanAnomalyInDegrees, ICalendar calendar)
		{
			var parentBody = parentEntity.GetComponent<OrbitalPositionComponent>();
			var mu = Constants.GravitationalConstant * (mass + parentBody.Mass);
			var angularVelocity = 2.0 * Math.PI / (periodInDays * 24.0 * 3600.0);
			var radius = Math.Pow(mu / (angularVelocity * angularVelocity), 1.0 / 3.0);

			var secondsFromEpoch = calendar.CreateTimePoint(2000, 1, 1).Tick * Constants.SecondsPerTick;
			var initialAngle = (secondsFromEpoch * angularVelocity) + meanAnomalyInDegrees / (2.0 * Math.PI);
			var relativePosition = new Point(radius * Math.Cos(initialAngle), radius * Math.Sin(initialAngle));

			var data = new UnpoweredStarSystemPositionData
			{
				AngularVelocity = angularVelocity,
				Mu = mu,
				OrbitalRadius = radius,
			};

			return new OrbitalPositionComponent(data)
			{
				Mass = mass,
				RelativePosition = relativePosition,
				Parent = parentEntity,
			};
		}

		public Entity Parent { get; set; }
		public Point RelativePosition { get; set; }
		public double Mass { get; set; }

		public Point GetAbsolutePosition()
		{
			return m_positionData.GetAbsolutePosition(this);
		}

		public Point GetAbsolutePositionAtTime(double ticks)
		{
			return m_positionData.GetAbsolutePositionAtTime(this, ticks);
		}

		public Point? GetInterceptPoint(Point interceptorPosition, double interceptorMaxSpeed)
		{
			return m_positionData.GetInterceptPoint(this, interceptorPosition, interceptorMaxSpeed);
		}

		public void EnsureValidity()
		{
			m_positionData.EnsureValidity(this);
		}

		public void MoveOneTick()
		{
			m_positionData.MoveOneTick(this);
		}

		private OrbitalPositionComponent(StarSystemPositionDataBase positionData)
		{
			m_positionData = positionData;
		}

		StarSystemPositionDataBase m_positionData;
	}
}
