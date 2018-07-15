using OneSmallStep.ECS;

namespace OneSmallStep.EntityViewModels
{
	public abstract class EntityViewModelBase : ViewModelBase
	{
		public Entity Entity { get; }

		public abstract void UpdateFromEntity();

		protected EntityViewModelBase(Entity entity)
		{
			Entity = entity;
		}
	}
}
