using System.Collections.Generic;
using System.Windows;
using GoldenAnvil.Utility.Logging;
using OneSmallStep.ECS;
using OneSmallStep.ECS.Components;

namespace OneSmallStep.UI.SystemMap
{
	public sealed class SystemMapViewModel : ViewModelBase
	{
		public SystemMapViewModel()
		{
			m_bodies = new List<ISystemBodyRenderer>();
			Scale = 1.0 / 1.2E12;
		}

		public Point Center
		{
			get
			{
				VerifyAccess();
				return m_center;
			}
			set
			{
				if (SetPropertyField(value, ref m_center))
					Log.Info($"Setting Center to ({Center.X}, {Center.Y})");
			}
		}

		public double Scale
		{
			get
			{
				VerifyAccess();
				return m_scale;
			}
			set
			{
				SetPropertyField(value, ref m_scale);
			}
		}

		public string CurrentDate
		{
			get
			{
				VerifyAccess();
				return m_currentDate;
			}
		}

		public IReadOnlyList<ISystemBodyRenderer> Bodies
		{
			get
			{
				VerifyAccess();
				return m_bodies;
			}
		}

		public void Update(string date, IReadOnlyList<ISystemBodyRenderer> entities)
		{
			var propertyNames = new List<string>();
			if (date != m_currentDate)
				propertyNames.Add(nameof(CurrentDate));
			propertyNames.Add(nameof(Bodies));
			using (ScopedPropertyChange(propertyNames.ToArray()))
			{
				m_currentDate = date;
				m_bodies = entities;
			}
		}

		public void GoToEntity(EntityId entityId, IEntityLookup entityLookup)
		{
			var entity = entityLookup.GetEntity(entityId);
			if (entity != null)
			{
				var position = entity.GetRequiredComponent<OrbitalPositionComponent>();
				Center = position.GetCurrentAbsolutePosition(entityLookup);
			}
		}

		private static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(SystemMapViewModel));

		string m_currentDate;
		IReadOnlyList<ISystemBodyRenderer> m_bodies;
		Point m_center;
		double m_scale;
	}
}
