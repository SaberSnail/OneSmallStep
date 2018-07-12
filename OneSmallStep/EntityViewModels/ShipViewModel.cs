using GoldenAnvil.Utility;
using OneSmallStep.ECS;
using OneSmallStep.ECS.Components;

namespace OneSmallStep.EntityViewModels
{
	public sealed class ShipViewModel : EntityViewModelBase
	{
		public ShipViewModel(Entity entity)
			: base(entity)
		{
		}

		public string Position
		{
			get
			{
				VerifyAccess();
				return m_position;
			}
			private set
			{
				SetPropertyField(nameof(Position), value, ref m_position);
			}
		}

		public override void UpdateFromEntity()
		{
			var body = Entity.GetComponent<PoweredAstronomicalBodyComponent>();
			var position = body?.AbsolutePosition;
			Position = position.HasValue ? "{0}, {1}".FormatCurrentUiCulture(position.Value.X, position.Value.Y) : null;
		}

		string m_position;
	}
}
