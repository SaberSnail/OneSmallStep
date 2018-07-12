using System.Windows;

namespace OneSmallStep.ECS.Components
{
	public sealed class UnpoweredAstronomicalBodyComponent : ComponentBase
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
			UnpoweredAstronomicalBodyComponent currentBody = this;
			while (true)
			{
				currentBody = currentBody.Parent?.GetComponent<UnpoweredAstronomicalBodyComponent>();
				if (currentBody == null)
					break;
				position.Offset(currentBody.RelativePosition.X, currentBody.RelativePosition.Y);
			}
			return position;
		}

		public Point GetAbsoluteOrbitCenterPosition()
		{
			return Parent?.GetComponent<UnpoweredAstronomicalBodyComponent>()?.GetAbsolutePosition() ?? RelativePosition;
		}
	}
}
