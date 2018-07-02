using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GoldenAnvil.Utility;
using JetBrains.Annotations;

namespace OneSmallStep.ECS
{
	public sealed class Entity
	{
		public Entity([NotNull] EntityManager entityManager, [NotNull] IEnumerable<ComponentBase> components)
		{
			m_components = components.ToDictionary(x => x.GetType(), x => x);
			m_componentKey = entityManager.CreateComponentKey(m_components.Values);
		}

		[NotNull]
		public IEnumerable<ComponentBase> Components
		{
			get { return m_components.Values; }
		}

		[NotNull]
		public BitArray ComponentKey
		{
			get { return m_componentKey; }
		}

		[CanBeNull]
		public T GetComponent<T>()
			where T : ComponentBase
		{
			return (T) m_components.GetValueOrDefault(typeof(T));
		}

		readonly Dictionary<Type, ComponentBase> m_components;

		BitArray m_componentKey;
	}
}
