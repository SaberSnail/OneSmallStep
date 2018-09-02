using System;
using System.Collections.Generic;
using System.Linq;
using GoldenAnvil.Utility;
using JetBrains.Annotations;

namespace OneSmallStep.ECS
{
	public sealed class Entity
	{
		public Entity([NotNull] IEntityLookup entityLookup)
		{
			Id = entityLookup.GetNextEntityId();
			m_entityLookup = entityLookup;
			m_components = new Dictionary<Type, ComponentBase>();
			ComponentKey = ComponentKey.Empty;
		}

		public Entity([NotNull] Entity that)
		{
			Id = that.Id;
			m_entityLookup = that.m_entityLookup;
			m_components = new Dictionary<Type, ComponentBase>();
			ComponentKey = ComponentKey.Empty;
			CloneFrom(that);
		}

		public EntityId Id { get; }

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
			ComponentKey = m_entityLookup.CreateComponentKey(m_components.Values);
		}

		public void CloneFrom(Entity that)
		{
			var thisComponentTypes = m_components.Keys.ToHashSet();
			var thatComponentTypes = that.m_components.Keys.ToHashSet();
			HashSet<Type> componentTypesInBoth = new HashSet<Type>();
			foreach (var type in thisComponentTypes.ToList())
			{
				if (thisComponentTypes.Contains(type))
				{
					thisComponentTypes.Remove(type);
					thatComponentTypes.Remove(type);
					componentTypesInBoth.Add(type);
				}
			}

			foreach (var typeToDelete in thisComponentTypes)
				m_components.Remove(typeToDelete);

			foreach (var typeToAdd in thatComponentTypes)
				m_components[typeToAdd] = that.m_components[typeToAdd].Clone();

			foreach (var typeToUpdate in componentTypesInBoth)
			{
				if (that.m_components[typeToUpdate].Version > m_components[typeToUpdate].Version)
					m_components[typeToUpdate] = that.m_components[typeToUpdate].Clone();
			}

			ComponentKey = that.ComponentKey;
		}

		readonly IEntityLookup m_entityLookup;
		readonly Dictionary<Type, ComponentBase> m_components;
	}
}
