using System;
using OneSmallStep.Time;

namespace OneSmallStep.ECS
{
	public sealed class GameData
	{
		public GameData()
		{
			m_rng = new Random();
		}

		public Random Random
		{
			get { return m_rng; }
		}

		public TimePoint CurrentDate { get; set; }

		Random m_rng;
	}
}
