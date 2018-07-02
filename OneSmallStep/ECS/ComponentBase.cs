namespace OneSmallStep.ECS
{
	public abstract class ComponentBase
	{
		public ComponentBase(Entity entity)
		{
			m_entity = entity;
		}

		protected Entity Entity
		{
			get { return m_entity; }
		}

		readonly Entity m_entity;
	}
}
