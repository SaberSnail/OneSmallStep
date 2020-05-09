using System;
using System.Collections.Generic;
using System.IO;
using GoldenAnvil.Utility;
using OneSmallStep.ECS;

namespace OneSmallStep.Utility
{
	public static class SystemDataFileUtility
	{
		public static IReadOnlyCollection<Entity> LoadEntities(string filename, IEntityLookup entityLookup, Random rng)
		{
			var entities = new Dictionary<string, Entity>();

			int lineNumber = 0;
			int asteroidCount = 0;

			var lines = File.ReadAllLines(filename);
			foreach (var line in lines)
			{
				void ThrowFormatException(string message)
				{
					throw new FormatException($"{filename}:{lineNumber} - {message}\n{line}");
				}

				lineNumber++;
				var parts = TrimComments(line).Split(new[] { ',' }, StringSplitOptions.None);
				if (parts.Length == 0 || (parts.Length == 1 && parts[0].Length == 0))
					continue;

				if (parts[0] == "S")
					CreateStarEntity(parts, entityLookup, entities, ThrowFormatException);
				else if (parts[0] == "B")
					CreateAsteroidBeltEntities(parts, ref asteroidCount, entityLookup, rng, entities, ThrowFormatException);
				else
					CreatePlanetEntity(parts, entityLookup, entities, ThrowFormatException);
			}

			return entities.Values;
		}

		private static void CreateStarEntity(string[] parts, IEntityLookup entityLookup, Dictionary<string, Entity> entities, Action<string> throwFormatException)
		{
			if (parts.Length != 4)
				throwFormatException($"Star data must have 4 fields, but found {parts.Length} fields.");

			var name = parts[1];
			var radius = double.Parse(parts[2]);
			var mass = double.Parse(parts[3]);

			var entity = EntityUtility.CreatePlanet(entityLookup, name, mass, radius);
			entities.Add(name, entity);
		}

		private static void CreatePlanetEntity(string[] parts, IEntityLookup entityLookup, Dictionary<string, Entity> entities, Action<string> throwFormatException)
		{
			if (parts.Length != 11)
				throwFormatException($"Planet data must have 11 fields, but found {parts.Length} fields.");

			var name = parts[1];
			var radius = double.Parse(parts[2]);
			var mass = double.Parse(parts[3]);
			if (mass < 0)
				mass = 1e12 * (4.0 / 3.0) * Math.PI * radius * radius * radius * mass;
			var parentId = entities[parts[4]].Id;
			var semiMajorAxis = double.Parse(parts[5]);
			var eccentricity = double.Parse(parts[6]);
			var longitudeOfPeriapsis = double.Parse(parts[7]);
			var meanAnomaly = double.Parse(parts[8]);
			var isRetrograde = int.Parse(parts[9]) != 0;
			var epoch = int.Parse(parts[10]);

			var entity = EntityUtility.CreatePlanet(entityLookup, name, parentId, mass, radius, semiMajorAxis, eccentricity, longitudeOfPeriapsis, meanAnomaly, isRetrograde, epoch);
			entities.Add(name, entity);
		}

		private static void CreateAsteroidBeltEntities(string[] parts, ref int asteroidCount, IEntityLookup entityLookup, Random rng, Dictionary<string, Entity> entities, Action<string> throwFormatException)
		{
			if (parts.Length != 7)
				throwFormatException($"Asteroid belt data must have 7 fields, but found {parts.Length} fields.");

			var count = int.Parse(parts[1]);
			var parentId = entities[parts[2]].Id;
			var minSemiMajorAxis = double.Parse(parts[3]);
			var maxSemiMajorAxis = double.Parse(parts[4]);
			var minEccentricity = double.Parse(parts[5]);
			var maxEccentricity = double.Parse(parts[6]);

			for (int i = 0; i < count; i++)
			{
				asteroidCount++;
				var name = $"Asteroid {asteroidCount}";
				var radius = 100.0;
				var mass = 1e12 * (4.0 / 3.0) * Math.PI * radius * radius * radius * 2600.0;
				var semiMajorAxis = rng.NextDouble(minSemiMajorAxis, maxSemiMajorAxis);
				var eccentricity = rng.NextDouble(minEccentricity, maxEccentricity);
				var longitudeOfPeriapsis = rng.NextDouble(0.0, 360.0);
				var meanAnomaly = rng.NextDouble(0.0, 360.0);
				var isRetrograde = rng.Next(0, 1000) == 0;
				var epoch = 2000;

				var entity = EntityUtility.CreatePlanet(entityLookup, name, parentId, mass, radius, semiMajorAxis, eccentricity, longitudeOfPeriapsis, meanAnomaly, isRetrograde, epoch);
				entities.Add(name, entity);
			}
		}

		private static string TrimComments(string text)
		{
			var commentIndex = text.IndexOf("//", StringComparison.OrdinalIgnoreCase);
			if (commentIndex != -1)
				return text.Substring(0, commentIndex);
			return text;
		}
	}
}
