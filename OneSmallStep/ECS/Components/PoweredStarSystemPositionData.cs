using System;
using System.Windows;
using GoldenAnvil.Utility.Logging;

namespace OneSmallStep.ECS.Components
{
	public sealed class PoweredStarSystemPositionData : StarSystemPositionDataBase
	{
		public Entity TargetEntity { get; set; }
		public Point? TargetPoint { get; set; }
		public double MaxSpeed { get; set; }

		public override void TrySetTarget(Entity target)
		{
			if (TargetEntity == null)
			{
				TargetEntity = target;
				TargetPoint = null;
			}
		}

		public override Point? TryGetTargetPoint() => TargetPoint;

		public override Point GetAbsolutePosition(OrbitalPositionComponent body)
		{
			return body.RelativePosition;
		}

		public override Point GetAbsolutePositionAtTime(OrbitalPositionComponent body, double ticks)
		{
			var absolutePosition = GetAbsolutePosition(body);
			if (TargetPoint.HasValue)
			{
				var vector = new Vector(absolutePosition.X, absolutePosition.Y);
				var targetVector = new Vector(TargetPoint.Value.X, TargetPoint.Value.Y);
				vector = targetVector - vector;
				vector.Normalize();
				vector = vector * MaxSpeed * ticks;
				absolutePosition += vector;
			}

			return absolutePosition;
		}

		public override void EnsureStartValidity(OrbitalPositionComponent body)
		{
			if (TargetEntity != null && TargetPoint == null)
			{
				var targetBody = TargetEntity.GetComponent<OrbitalPositionComponent>();
				if (targetBody != null)
					TargetPoint = targetBody.GetInterceptPoint(GetAbsolutePosition(body), MaxSpeed);
			}
		}

		public override void EnsureEndValidity(OrbitalPositionComponent body)
		{
			if (TargetEntity != null && TargetPoint == null)
			{
				var targetBody = TargetEntity.GetComponent<OrbitalPositionComponent>();
				var targetPosition = targetBody.GetAbsolutePosition();
				Log.Info($"Target body is at ({targetPosition.X}, {targetPosition.Y}).");
				var currentPosition = GetAbsolutePosition(body);
				if (Math.Abs(currentPosition.X - targetPosition.X) > 1 || Math.Abs(currentPosition.Y - targetPosition.Y) > 1)
					Log.Info("Current position does not match target position.");
				else
					TargetEntity = null;
			}
		}

		public override void MoveOneTick(OrbitalPositionComponent body)
		{
			if (TargetPoint.HasValue)
			{
				var absolutePosition = GetAbsolutePosition(body);
				var vector = new Vector(absolutePosition.X, absolutePosition.Y);
				var targetVector = new Vector(TargetPoint.Value.X, TargetPoint.Value.Y);
				vector = targetVector - vector;
				if ((MaxSpeed * MaxSpeed) > vector.LengthSquared)
				{
					body.RelativePosition = TargetPoint.Value;
					Log.Info($"Reached target point ({TargetPoint.Value.X}, {TargetPoint.Value.Y})");
					TargetPoint = null;
				}
				else
				{
					vector.Normalize();
					vector = vector * MaxSpeed;
					body.RelativePosition = absolutePosition + vector;
				}
			}
		}

		public override Point? GetInterceptPoint(OrbitalPositionComponent body, Point interceptorPosition, double interceptorMaxSpeed)
		{
			return null;
		}

		static readonly ILogSource Log = LogManager.CreateLogSource(nameof(PoweredStarSystemPositionData));
	}
}
