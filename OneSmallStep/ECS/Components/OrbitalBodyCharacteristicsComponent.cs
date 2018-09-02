namespace OneSmallStep.ECS.Components
{
	public sealed class OrbitalBodyCharacteristicsComponent : ComponentBase
	{
		public OrbitalBodyCharacteristicsComponent()
		{
		}

		public double Mass
		{
			get => m_mass;
			set => SetPropertyField(value, ref m_mass);
		}

		public double Radius
		{
			get => m_radius;
			set => SetPropertyField(value, ref m_radius);
		}

		public override ComponentBase Clone()
		{
			return new OrbitalBodyCharacteristicsComponent(this);
		}

		private OrbitalBodyCharacteristicsComponent(OrbitalBodyCharacteristicsComponent that)
		: base(that)
		{
			m_mass = that.m_mass;
			m_radius = that.m_radius;
		}

		double m_mass;
		double m_radius;
	}
}
