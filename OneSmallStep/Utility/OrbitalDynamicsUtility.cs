using System;
using System.Windows;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Logging;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.ECS.Components;

namespace OneSmallStep.Utility
{
	public static class OrbitalDynamicsUtility
	{
		public static Point GetInterceptPointForTarget(PoweredAstronomicalBodyComponent ship, UnpoweredAstronomicalBodyComponent targetBody)
		{
			var targetOribitCenter = targetBody.GetAbsoluteOrbitCenterPosition();
			var targetAbsolutePosition = targetOribitCenter;
			targetAbsolutePosition.Offset(targetBody.RelativePosition.X, targetBody.RelativePosition.Y);

			var shipOrbitalRadius = ship.AbsolutePosition.DistanceTo(targetAbsolutePosition);
			var shortestDistance = Math.Abs(shipOrbitalRadius - targetBody.OrbitalRadius.Value);
			var minTimeInTicks = shortestDistance / ship.Speed;
			var maxTimeInTicks = (targetBody.OrbitalRadius.Value * 2.0 / ship.Speed) + (shipOrbitalRadius > targetBody.OrbitalRadius.Value ? minTimeInTicks : -minTimeInTicks);
			// TODO: use time for first time planet is closest to ship starting position after minTimeInTicks if it results in a smaller maxTimeInTicks

			var maxError = 0.25;
			var timeInTicks = minTimeInTicks;
			while (maxTimeInTicks - minTimeInTicks >= maxError)
			{
				timeInTicks = (minTimeInTicks + maxTimeInTicks) / 2.0;
				var error = GetError(targetAbsolutePosition, targetOribitCenter, targetBody.OrbitalRadius.Value, targetBody.AngularVelocity.Value, ship.AbsolutePosition, ship.Speed, timeInTicks);
				if (Math.Abs(error) < maxError)
					break;

				if (error * GetError(targetAbsolutePosition, targetOribitCenter, targetBody.OrbitalRadius.Value, targetBody.AngularVelocity.Value, ship.AbsolutePosition, ship.Speed, minTimeInTicks) < 0)
					maxTimeInTicks = timeInTicks;
				else
					minTimeInTicks = timeInTicks;
			}
			
			var targetPoint = GetPositionAtTime(targetAbsolutePosition, targetOribitCenter, targetBody.OrbitalRadius.Value, targetBody.AngularVelocity.Value, Math.Ceiling(timeInTicks));
			Log.Info($"Targeting point ({targetPoint.X}, {targetPoint.Y}), will reach in {timeInTicks} ticks.");
			return targetPoint;
		}

		private static double GetError(Point targetInitialPosition, Point targetOrbitCenter, double targetOrbitalRadius, double targetAngularVelocity, Point shipPosition, double shipSpeedPerTick, double tick)
		{
			var targetPosition = GetPositionAtTime(targetInitialPosition, targetOrbitCenter, targetOrbitalRadius, targetAngularVelocity, tick);
			var dx = targetPosition.X - shipPosition.X;
			var dy = targetPosition.Y - shipPosition.Y;

			return ((dx * dx) + (dy * dy)) / ((shipSpeedPerTick * shipSpeedPerTick)) - (tick * tick);
		}

		private static Point GetPositionAtTime(Point initialPosition, Point orbitCenter, double orbitalRadius, double angularVelocityPerTick, double tick)
		{
			var currentAngle = MathUtility.DegreesToRadians(Vector.AngleBetween(new Vector(1, 0), new Vector(initialPosition.X, initialPosition.Y)));
			var angle = currentAngle + (angularVelocityPerTick * tick * Constants.SecondsPerTick);
			//var angle = Math.Atan2(initialPosition.X - orbitCenter.X, initialPosition.Y - orbitCenter.Y) + (tick * angularVelocityPerTick * Constants.SecondsPerTick);
			return new Point(Math.Cos(angle) * orbitalRadius + orbitCenter.X, Math.Sin(angle) * orbitalRadius + orbitCenter.Y);
		}

		static readonly ILogSource Log = LogManager.CreateLogSource(nameof(OrbitalDynamicsUtility));
	}
}
