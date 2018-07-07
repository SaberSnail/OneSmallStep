using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace OneSmallStep.ECS
{
	public sealed class EntityManager
	{
		public EntityManager()
		{
			m_state = State.StartingUp;
			m_componentIdToType = new Dictionary<int, Type>();
			m_componentTypeToId = new Dictionary<Type, int>();
			m_entities = new List<Entity>();
		}

		public void RegisterComponent<T>()
			where T: ComponentBase
		{
			if (m_state != State.StartingUp)
				throw new InvalidOperationException("This code may only be called during startup.");

			Type type = typeof(T);
			int newId = m_componentIdToType.Count;
			m_componentIdToType.Add(newId, type);
			m_componentTypeToId.Add(type, newId);
		}

		public void SetStartupFinished()
		{
			if (m_state != State.StartingUp)
				throw new InvalidOperationException("This code may only be called during startup.");

			m_state = State.Started;
		}

		[NotNull]
		public IEnumerable<Entity> GetEntitiesMatchingKey(ComponentKey componentKey)
		{
			return m_entities.Where(x => (x.ComponentKey & componentKey) == componentKey);
		}

		public void RegisterEntity([NotNull] Entity entity)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			m_entities.Add(entity);
		}

		[NotNull]
		public ComponentKey CreateComponentKey([NotNull] IEnumerable<ComponentBase> components)
		{
			return CreateComponentKey(components.Select(x => x.GetType()).ToArray());
		}

		public ComponentKey CreateComponentKey(params Type[] componentTypes)
		{
			var key = ComponentKey.Empty;
			foreach (var type in componentTypes)
			{
				int id;
				if (!m_componentTypeToId.TryGetValue(type, out id))
					throw new InvalidOperationException($"Specified type has not been registered: {type.Name}");
				key |= ComponentKey.CreateFromComponentId(id);
			}
			return key;
		}

		private enum State
		{
			StartingUp,
			Started,
		}

		readonly Dictionary<int, Type> m_componentIdToType;
		readonly Dictionary<Type, int> m_componentTypeToId;
		readonly List<Entity> m_entities;

		State m_state;
	}
}
