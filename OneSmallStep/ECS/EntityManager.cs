using System;
using System.Collections;
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
		public IEnumerable<Entity> GetEntitiesMatchingKey(BitArray componentKey)
		{
			return m_entities.Where(x => x.ComponentKey.And(componentKey) == componentKey);
		}

		[NotNull]
		public BitArray CreateComponentKey([NotNull] IEnumerable<ComponentBase> components)
		{
			return CreateComponentKey(components.Select(x => x.GetType()).ToArray());
		}

		[NotNull]
		public BitArray CreateComponentKey(params Type[] componentTypes)
		{
			if (m_state != State.Started)
				throw new InvalidOperationException("This code may only be called after startup.");

			BitArray bits = new BitArray(m_componentIdToType.Count);
			foreach (Type componentType in componentTypes)
			{
				int id;
				if (!m_componentTypeToId.TryGetValue(componentType, out id))
					throw new InvalidOperationException(string.Format("Specified type has not been registered: {0}", componentType.Name));
				bits.Set(id, true);
			}

			return bits;
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
