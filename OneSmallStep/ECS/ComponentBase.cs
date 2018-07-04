namespace OneSmallStep.ECS
{
	public abstract class ComponentBase
	{
		public ComponentBase(Entity entity)
		{
			Entity = entity;
		}

		protected Entity Entity { get; }
	}
}
