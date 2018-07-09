using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GoldenAnvil.Utility.Logging;
using OneSmallStep.ECS;

namespace OneSmallStep.SystemMap
{
	public interface ISystemMapEntity
	{ }
	public sealed class SystemMapViewModel : ViewModelBase
	{
		public SystemMapViewModel()
		{
			m_entities = new List<Entity>();
			Scale = 1.0 / 2.5E11;
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
				if (SetPropertyField(nameof(Center), value, ref m_center))
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
				SetPropertyField(nameof(Scale), value, ref m_scale);
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

		public IReadOnlyList<Entity> Entities
		{
			get
			{
				VerifyAccess();
				return m_entities;
			}
		}

		public void Update(string date, IReadOnlyList<Entity> entities)
		{
			var propertyNames = new List<string>();
			if (date != m_currentDate)
				propertyNames.Add(nameof(CurrentDate));
			if (!entities.SequenceEqual(m_entities))
				propertyNames.Add(nameof(Entities));
			using (ScopedPropertyChange(propertyNames.ToArray()))
			{
				m_currentDate = date;
				m_entities = entities;
			}
		}

		static readonly ILogSource Log = LogManager.CreateLogSource(nameof(SystemMapViewModel));

		string m_currentDate;
		IReadOnlyList<Entity> m_entities;
		Point m_center;
		double m_scale;
	}
}
