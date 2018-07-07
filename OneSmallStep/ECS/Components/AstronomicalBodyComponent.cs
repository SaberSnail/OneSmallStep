using System.Windows;

namespace OneSmallStep.ECS.Components
{
	public sealed class AstronomicalBodyComponent : ComponentBase
	{
		public double Mass { get; set; }
		public Entity Parent { get; set; }

		public Point RelativePosition { get; set; }

		public double? Mu { get; set; }
		public double? OrbitalRadius { get; set; }
		public double? AngularVelocity { get; set; }

		public Point GetAbsolutePosition()
		{
			var position = RelativePosition;
			AstronomicalBodyComponent currentBody = this;
			while (true)
			{
				currentBody = currentBody.Parent?.GetComponent<AstronomicalBodyComponent>();
				if (currentBody == null)
					break;
				position.Offset(currentBody.RelativePosition.X, currentBody.RelativePosition.Y);
			}
			return position;
		}
	}
}
