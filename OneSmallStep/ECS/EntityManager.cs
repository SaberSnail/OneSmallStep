using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Windows;

namespace OneSmallStep.ECS
{
	public sealed class EntityManager
	{
		public EntityManager()
		{
			m_dispatcher = Dispatcher.CurrentDispatcher;
			m_componentIdToType = new Dictionary<int, Type>();
			m_componentTypeToId = new Dictionary<Type, int>();
			m_displayLookup = new EntityLookup(this);
			m_processingLookup = new EntityLookup(this);
			m_nextEntityId = 1;
			m_state = State.StartingUp;
		}

		public IEntityLookup DisplayEntityLookup
		{
			get
			{
				m_dispatcher.VerifyCurrent();
				return m_displayLookup;
			}
		}

		public IEntityLookup ProcessingEntityLookup
		{
			get
			{
				m_dispatcher.VerifyNotCurrent();
				return m_processingLookup;
			}
		}

		public Scope StartupScope()
		{
			if (m_state != State.StartingUp)
				throw new InvalidOperationException("This code may only be called during startup.");
			return Scope.Create(() => m_state = State.Started);
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

		public void SwapDisplayWithProcessing()
		{
			m_dispatcher.VerifyCurrent();
			EntityLookup.SwapEntities(m_displayLookup, m_processingLookup);
		}

		public void UpdateProcessingEntitiesFromDisplay()
		{
			m_dispatcher.VerifyNotCurrent();
			EntityLookup.UpdateEntities(m_displayLookup, m_processingLookup);
		}

		private EntityId GetNextEntityId()
		{
			return new EntityId(unchecked((uint) Interlocked.Increment(ref m_nextEntityId)));
		}

		private sealed class EntityLookup : IEntityLookup
		{
			public static void SwapEntities(EntityLookup left, EntityLookup right)
			{
				var tempEntities = left.m_entities;
				left.m_entities = right.m_entities;
				right.m_entities = tempEntities;
			}

			public static void UpdateEntities(EntityLookup source, EntityLookup destination)
			{
				var sourceIds = source.m_entities.Keys.ToHashSet();
				var idsToAdd = new HashSet<EntityId>();
				var idsToDelete = destination.m_entities.Keys.ToHashSet();
				var idsToUpdate = new HashSet<EntityId>();

				foreach (var id in sourceIds)
				{
					if (idsToDelete.Contains(id))
					{
						idsToUpdate.Add(id);
						idsToDelete.Remove(id);
					}
					else
					{
						idsToAdd.Add(id);
					}
				}

				foreach (var id in idsToUpdate)
				{
					var sourceEntity = source.GetEntity(id);
					var destinationEntity = destination.GetEntity(id);
					destinationEntity.CloneFrom(sourceEntity);
				}

				foreach (var id in idsToDelete)
					destination.m_entities.Remove(id);

				foreach (var id in idsToAdd)
				{
					var entity = new Entity(source.GetEntity(id));
					destination.m_entities.Add(entity.Id, entity);
				}
			}

			public EntityLookup(EntityManager manager)
			{
				m_manager = manager;
				m_entities = new Dictionary<EntityId, Entity>();
			}

			public EntityId GetNextEntityId()
			{
				return m_manager.GetNextEntityId();
			}

			public IReadOnlyList<Entity> GetEntitiesMatchingKey(ComponentKey componentKey)
			{
				return m_entities.Values.Where(x => (x.ComponentKey & componentKey) == componentKey).ToList();
			}

			public IReadOnlyList<Entity> GetEntitiesMatchingKeys(params ComponentKey[] componentKey)
			{
				return m_entities.Values.Where(x => componentKey.Any(key => (x.ComponentKey & key) == key)).ToList();
			}

			public Entity GetEntity(EntityId entityId)
			{
				return m_entities.GetValueOrDefault(entityId);
			}

			public void RegisterEntity(Entity entity)
			{
				if (entity == null)
					throw new ArgumentNullException(nameof(entity));

				m_entities.Add(entity.Id, entity);
			}

			public ComponentKey CreateComponentKey(IEnumerable<ComponentBase> components)
			{
				return CreateComponentKey(components.Select(x => x.GetType()).ToArray());
			}

			public ComponentKey CreateComponentKey<T>()
			{
				return CreateComponentKey(typeof(T));
			}

			public ComponentKey CreateComponentKey(params Type[] componentTypes)
			{
				var key = ComponentKey.Empty;
				foreach (var type in componentTypes)
				{
					if (!m_manager.m_componentTypeToId.TryGetValue(type, out int id))
						throw new InvalidOperationException($"Specified type has not been registered: {type.Name}");
					key |= ComponentKey.CreateFromComponentId(id);
				}
				return key;
			}

			readonly EntityManager m_manager;

			Dictionary<EntityId, Entity> m_entities;
		}

		private enum State
		{
			StartingUp,
			Started,
		}

		readonly Dispatcher m_dispatcher;
		readonly Dictionary<int, Type> m_componentIdToType;
		readonly Dictionary<Type, int> m_componentTypeToId;
		readonly EntityLookup m_displayLookup;
		readonly EntityLookup m_processingLookup;

		State m_state;
		int m_nextEntityId;
	}
}
