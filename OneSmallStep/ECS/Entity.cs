using System;
using System.Collections.Generic;
using GoldenAnvil.Utility;
using JetBrains.Annotations;

namespace OneSmallStep.ECS
{
	public sealed class Entity
	{
		public Entity([NotNull] EntityManager entityManager)
		{
			m_entityManager = entityManager;
			m_components = new Dictionary<Type, ComponentBase>();
			ComponentKey = entityManager.CreateComponentKey(m_components.Values);
		}

		public ComponentKey ComponentKey { get; private set; }

		[CanBeNull]
		public T GetOptionalComponent<T>()
			where T : ComponentBase
		{
			return (T) m_components.GetValueOrDefault(typeof(T));
		}

		[NotNull]
		public T GetRequiredComponent<T>()
			where T : ComponentBase
		{
			return (T) m_components[typeof(T)];
		}

		public void AddComponent(ComponentBase component)
		{
			m_components.Add(component.GetType(), component);
			ComponentKey = m_entityManager.CreateComponentKey(m_components.Values);
		}

		readonly EntityManager m_entityManager;
		readonly Dictionary<Type, ComponentBase> m_components;
	}
}
