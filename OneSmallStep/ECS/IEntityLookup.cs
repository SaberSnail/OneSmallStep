using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OneSmallStep.ECS
{
	public interface IEntityLookup
	{
		EntityId GetNextEntityId();

		[NotNull]
		IReadOnlyList<Entity> GetEntitiesMatchingKey(ComponentKey componentKey);

		[NotNull]
		IReadOnlyList<Entity> GetEntitiesMatchingKeys(params ComponentKey[] componentKey);

		Entity GetEntity(EntityId entityId);

		void RegisterEntity([NotNull] Entity entity);

		ComponentKey CreateComponentKey([NotNull] IEnumerable<ComponentBase> components);

		ComponentKey CreateComponentKey<T>();

		ComponentKey CreateComponentKey(params Type[] componentTypes);
	}
}
