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
			TargetEntity = target;
			TargetPoint = null;
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

		public override void EnsureValidity(OrbitalPositionComponent body)
		{
			if (TargetEntity != null && TargetPoint == null)
			{
				var targetBody = TargetEntity.GetComponent<OrbitalPositionComponent>();
				if (targetBody != null)
					TargetPoint = targetBody.GetInterceptPoint(GetAbsolutePosition(body), MaxSpeed);
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
					Log.Info($"Reached target point ({TargetPoint.Value.X}, {TargetPoint.Value.Y})");
					body.RelativePosition = TargetPoint.Value;
					TargetPoint = null;
					TargetEntity = null;
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
