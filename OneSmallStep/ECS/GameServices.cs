using System;

namespace OneSmallStep.ECS
{
	public sealed class GameServices
	{
		public GameServices()
		{
			RandomNumberGenerator = new Random(0);
		}

		public Random RandomNumberGenerator { get; }
	}
}
