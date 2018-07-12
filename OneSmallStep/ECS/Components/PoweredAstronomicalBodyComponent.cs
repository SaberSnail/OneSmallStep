using System.Windows;

namespace OneSmallStep.ECS.Components
{
	public sealed class PoweredAstronomicalBodyComponent : ComponentBase
	{
		public double Speed { get; set; }
		public Point AbsolutePosition { get; set; }
		public Point? TargetPoint { get; set; }
		public Entity TargetEntity { get; set; }
	}
}
