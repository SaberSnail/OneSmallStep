﻿using System;
using OneSmallStep.ECS.Systems;

namespace OneSmallStep.ECS
{
	public sealed class GameServices : IDisposable
	{
		public GameServices()
		{
			RandomNumberGenerator = new Random();
			Processor = CreateGameProcessor();
		}

		public Random RandomNumberGenerator { get; }

		public GameProcessor Processor { get; }

		public void Dispose()
		{
			Processor.Dispose();
		}

		private GameProcessor CreateGameProcessor()
		{
			var processor = new GameProcessor();

			processor.RegisterSystem(new StarSystemMovementSystem());
			processor.RegisterSystem(new PopulationGrowthSystem());
			processor.RegisterSystem(new ShipyardSystem());

			return processor;
		}
	}
}
