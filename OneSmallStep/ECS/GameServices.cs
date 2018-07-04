using System;

namespace OneSmallStep.ECS
{
	public sealed class GameServices
	{
		public GameServices()
		{
			RandomNumberGenerator = new Random();
		}

		public Random RandomNumberGenerator { get; }
	}
}
