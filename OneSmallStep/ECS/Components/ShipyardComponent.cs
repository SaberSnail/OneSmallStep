namespace OneSmallStep.ECS.Components
{
	public sealed class ShipyardComponent : ComponentBase
	{
		public ShipyardComponent()
			: base()
		{
		}

		public override ComponentBase Clone()
		{
			return new ShipyardComponent(this);
		}

		private ShipyardComponent(ShipyardComponent that)
			: base(that)
		{
		}
	}
}
