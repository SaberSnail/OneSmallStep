using System;
using System.Windows;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Logging;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.Utility;

namespace OneSmallStep.ECS.Components
{
	public sealed class UnpoweredStarSystemPositionData : StarSystemPositionDataBase
	{
		public double? Mu { get; set; }
		public double? OrbitalRadius { get; set; }
		public double? AngularVelocity { get; set; }

		public override Point GetAbsolutePosition(OrbitalPositionComponent body)
		{
			var position = body.RelativePosition;
			var parentPosition = body.Parent?.GetComponent<OrbitalPositionComponent>()?.GetAbsolutePosition();
			if (parentPosition != null)
				position.Offset(parentPosition.Value.X, parentPosition.Value.Y);
			return position;
		}

		public override Point GetAbsolutePositionAtTime(OrbitalPositionComponent body, double ticks)
		{
			var absolutePosition = body.RelativePosition;
			var parentPosition = body.Parent?.GetComponent<OrbitalPositionComponent>()?.GetAbsolutePositionAtTime(ticks);
			if (parentPosition != null)
			{
				var currentAngle = MathUtility.DegreesToRadians(Vector.AngleBetween(new Vector(1, 0), new Vector(absolutePosition.X, absolutePosition.Y)));
				var angle = currentAngle + (AngularVelocity.Value * ticks * Constants.SecondsPerTick);
				//var angle = Math.Atan2(initialPosition.X - orbitCenter.X, initialPosition.Y - orbitCenter.Y) + (tick * angularVelocityPerTick * Constants.SecondsPerTick);
				return new Point(Math.Cos(angle) * OrbitalRadius.Value + parentPosition.Value.X, Math.Sin(angle) * OrbitalRadius.Value + parentPosition.Value.Y);
			}

			return absolutePosition;
		}

		public override void EnsureValidity(OrbitalPositionComponent body)
		{
			var parentBody = body.Parent?.GetComponent<OrbitalPositionComponent>();

			if (parentBody != null)
			{
				if (!Mu.HasValue)
					Mu = Constants.GravitationalConstant * (body.Mass + parentBody.Mass);
				if (!OrbitalRadius.HasValue)
					OrbitalRadius = new Vector(body.RelativePosition.X, body.RelativePosition.Y).Length;
				if (!AngularVelocity.HasValue)
					AngularVelocity = Math.Sqrt(Math.Pow(OrbitalRadius.Value, 3.0) / Mu.Value);
			}
		}

		public override void MoveOneTick(OrbitalPositionComponent body)
		{
			var parentBody = body.Parent?.GetComponent<OrbitalPositionComponent>();

			if (parentBody != null)
			{
				var currentAngle = MathUtility.DegreesToRadians(Vector.AngleBetween(new Vector(1, 0), new Vector(body.RelativePosition.X, body.RelativePosition.Y)));
				var newAngle = currentAngle + AngularVelocity.Value * Constants.SecondsPerTick;
				body.RelativePosition = new Point(OrbitalRadius.Value * Math.Cos(newAngle), OrbitalRadius.Value * Math.Sin(newAngle));
			}
		}

		public override Point? GetInterceptPoint(OrbitalPositionComponent body, Point interceptorPosition, double interceptorMaxSpeed)
		{
			if (body.Parent == null)
				return GetAbsolutePosition(body);

			var targetOribitCenter = body.Parent.GetComponent<OrbitalPositionComponent>().GetAbsolutePosition();
			var targetAbsolutePosition = targetOribitCenter;
			targetAbsolutePosition.Offset(body.RelativePosition.X, body.RelativePosition.Y);

			var shipOrbitalRadius = interceptorPosition.DistanceTo(targetAbsolutePosition);
			var shortestDistance = Math.Abs(shipOrbitalRadius - OrbitalRadius.Value);
			var minTimeInTicks = shortestDistance / interceptorMaxSpeed;
			var maxTimeInTicks = (OrbitalRadius.Value * 2.0 / interceptorMaxSpeed) + (shipOrbitalRadius > OrbitalRadius.Value ? minTimeInTicks : -minTimeInTicks);
			// TODO: use time for first time planet is closest to ship starting position after minTimeInTicks if it results in a smaller maxTimeInTicks

			var maxError = 0.25;
			var timeInTicks = minTimeInTicks;
			while (maxTimeInTicks - minTimeInTicks >= maxError)
			{
				timeInTicks = (minTimeInTicks + maxTimeInTicks) / 2.0;
				var error = GetError(body, interceptorPosition, interceptorMaxSpeed, timeInTicks);
				if (Math.Abs(error) < maxError)
					break;

				if (error * GetError(body, interceptorPosition, interceptorMaxSpeed, minTimeInTicks) < 0)
					maxTimeInTicks = timeInTicks;
				else
					minTimeInTicks = timeInTicks;
			}

			var targetPoint = GetAbsolutePositionAtTime(body, Math.Ceiling(timeInTicks));
			Log.Info($"Targeting point ({targetPoint.X}, {targetPoint.Y}), will reach in {timeInTicks} ticks.");
			return targetPoint;
		}

		private double GetError(OrbitalPositionComponent body, Point interceptorPosition, double interceptorSpeed, double tick)
		{
			var targetPosition = GetAbsolutePositionAtTime(body, tick);
			var dx = targetPosition.X - interceptorPosition.X;
			var dy = targetPosition.Y - interceptorPosition.Y;

			return (((dx * dx) + (dy * dy)) / (interceptorSpeed * interceptorSpeed)) - (tick * tick);
		}

		static readonly ILogSource Log = LogManager.CreateLogSource(nameof(UnpoweredStarSystemPositionData));
	}
}
