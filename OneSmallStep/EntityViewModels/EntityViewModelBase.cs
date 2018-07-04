using OneSmallStep.ECS;

namespace OneSmallStep.EntityViewModels
{
	public abstract class EntityViewModelBase : ViewModelBase
	{
		public abstract void UpdateFromEntity();

		protected EntityViewModelBase(Entity entity)
		{
			Entity = entity;
		}

		protected Entity Entity { get; }
	}
}
