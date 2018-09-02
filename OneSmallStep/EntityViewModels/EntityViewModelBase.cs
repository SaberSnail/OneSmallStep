using OneSmallStep.ECS;

namespace OneSmallStep.EntityViewModels
{
	public abstract class EntityViewModelBase : ViewModelBase
	{
		public EntityId EntityId { get; }

		public abstract void UpdateFromEntity(IEntityLookup entityLookup);

		protected EntityViewModelBase(EntityId entityId)
		{
			EntityId = entityId;
		}
	}
}
