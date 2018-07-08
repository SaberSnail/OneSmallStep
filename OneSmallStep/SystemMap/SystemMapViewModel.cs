using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneSmallStep.ECS;
using OneSmallStep.Time;

namespace OneSmallStep.SystemMap
{
	public sealed class SystemMapViewModel : ViewModelBase
	{
		public SystemMapViewModel()
		{
			m_entities = new List<Entity>();
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

		string m_currentDate;
		IReadOnlyList<Entity> m_entities;
	}
}
