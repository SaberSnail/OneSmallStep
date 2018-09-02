namespace OneSmallStep.ECS.Components
{
	public sealed class OrbitalUnitDesignComponent : ComponentBase
	{
		public OrbitalUnitDesignComponent()
		{
		}

		public double MaxSpeedPerTick
		{
			get => m_maxSpeedPerTick;
			set => SetPropertyField(value, ref m_maxSpeedPerTick);
		}

		public override ComponentBase Clone()
		{
			return new OrbitalUnitDesignComponent(this);
		}

		private OrbitalUnitDesignComponent(OrbitalUnitDesignComponent that)
			: base(that)
		{
			m_maxSpeedPerTick = that.m_maxSpeedPerTick;
		}

		double m_maxSpeedPerTick;
	}
}
