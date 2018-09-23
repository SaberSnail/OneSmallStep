namespace OneSmallStep.ECS.Components
{
	public sealed class InformationComponent : ComponentBase
	{
		public InformationComponent()
		{
		}

		public string Name
		{
			get => m_name;
			set => SetPropertyField(value, ref m_name);
		}

		public override ComponentBase Clone()
		{
			return new InformationComponent(this);
		}

		private InformationComponent(InformationComponent that)
			: base(that)
		{
			m_name = that.m_name;
		}

		string m_name;
	}
}
