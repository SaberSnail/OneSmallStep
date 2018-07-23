using System.Collections.Generic;
using System.Linq;
using OneSmallStep.ECS.Components;

namespace OneSmallStep.ECS.Systems
{
	public sealed class StarSystemMovementSystem : SystemBase
	{
		public StarSystemMovementSystem(GameData gameData)
			: base(gameData)
		{
		}

		protected override ComponentKey GetRequiredComponentsKey()
		{
			return GameData.EntityManager.CreateComponentKey<OrbitalPositionComponent>();
		}

		protected override void ProcessTick(IEnumerable<Entity> entities)
		{
			var entitiesList = entities.ToList().AsReadOnly();

			foreach (var entity in entitiesList)
			{
				var body = entity.GetComponent<OrbitalPositionComponent>();
				body.EnsureStartValidity();
			}

			foreach (var entity in entitiesList)
			{
				var body = entity.GetComponent<OrbitalPositionComponent>();
				body.MoveOneTick();
			}

			foreach (var entity in entitiesList)
			{
				var body = entity.GetComponent<OrbitalPositionComponent>();
				body.EnsureEndValidity();
			}
		}
	}
}