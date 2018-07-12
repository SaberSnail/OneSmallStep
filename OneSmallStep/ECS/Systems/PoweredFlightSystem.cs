using System.Windows;
using GoldenAnvil.Utility.Logging;
using OneSmallStep.ECS.Components;
using OneSmallStep.Utility;

namespace OneSmallStep.ECS.Systems
{
	public sealed class PoweredFlightSystem : SystemBase
	{
		public PoweredFlightSystem(GameData gameData) : base(gameData)
		{
		}

		protected override ComponentKey GetRequiredComponentsKey()
		{
			return GameData.EntityManager.CreateComponentKey(typeof(PoweredAstronomicalBodyComponent));
		}

		protected override void ProcessTick(Entity entity)
		{
			var body = entity.GetComponent<PoweredAstronomicalBodyComponent>();

			if (body.TargetEntity != null && body.TargetPoint == null)
			{
				var targetBody = body.TargetEntity.GetComponent<UnpoweredAstronomicalBodyComponent>();
				if (targetBody != null)
				{
					body.TargetPoint = OrbitalDynamicsUtility.GetInterceptPointForTarget(body, targetBody);
				}
			}

			if (body.TargetPoint != null)
			{
				var vector = new Vector(body.AbsolutePosition.X, body.AbsolutePosition.Y);
				var targetVector = new Vector(body.TargetPoint.Value.X, body.TargetPoint.Value.Y);
				vector = targetVector - vector;
				if ((body.Speed * body.Speed) > vector.LengthSquared)
				{
					Log.Info($"Reached target point ({body.TargetPoint.Value.X}, {body.TargetPoint.Value.Y})");
					body.AbsolutePosition = body.TargetPoint.Value;
					body.TargetPoint = null;
					body.TargetEntity = null;
				}
				else
				{
					vector.Normalize();
					vector = vector * body.Speed;
					body.AbsolutePosition = body.AbsolutePosition + vector;
				}
			}
		}

		static readonly ILogSource Log = LogManager.CreateLogSource(nameof(PoweredFlightSystem));
	}
}
