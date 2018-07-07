using System;
using System.Windows;
using GoldenAnvil.Utility;
using OneSmallStep.ECS.Components;
using OneSmallStep.Time;
using OneSmallStep.Utility;

namespace OneSmallStep.ECS.Systems
{
	public sealed class OrbitalDynamicsSystem : SystemBase
	{
		public static AstronomicalBodyComponent CreateComponent(Entity parentEntity, double mass, double periodInDays, double meanAnomalyInDegrees, ICalendar calendar)
		{
			var parentBody = parentEntity?.GetComponent<AstronomicalBodyComponent>();
			if (parentBody == null)
				return new AstronomicalBodyComponent { Mass = mass };

			var mu = c_gravitationalConstant * (mass + parentBody.Mass);
			var angularVelocity = 2.0 * Math.PI / (periodInDays * 24.0 * 3600.0);
			var radius = Math.Pow(mu / (angularVelocity * angularVelocity), 1.0 / 3.0);

			var secondsFromEpoch = StandardCalendar.CreateTimePoint(2000, 1, 1, Constants.TicksPerDay).Tick * Constants.SecondsPerTick;
			var initialAngle = (secondsFromEpoch * angularVelocity) + meanAnomalyInDegrees / (2.0 * Math.PI);
			var relativePosition = new Point(radius * Math.Cos(initialAngle), radius * Math.Sin(initialAngle));

			return new AstronomicalBodyComponent
			{
				Mass = mass,
				Parent = parentEntity,
				Mu = mu,
				AngularVelocity = angularVelocity,
				OrbitalRadius = radius,
				RelativePosition = relativePosition,
			};
		}

		public OrbitalDynamicsSystem(GameData gameData) : base(gameData)
		{
		}

		protected override ComponentKey GetRequiredComponentsKey()
		{
			return GameData.EntityManager.CreateComponentKey(typeof(AstronomicalBodyComponent));
		}

		protected override void ProcessTick(Entity entity)
		{
			var body = entity.GetComponent<AstronomicalBodyComponent>();
			var parentBody = body.Parent?.GetComponent<AstronomicalBodyComponent>();

			if (parentBody != null)
			{
				if (!body.Mu.HasValue)
					body.Mu = c_gravitationalConstant * (body.Mass + parentBody.Mass);
				if (!body.OrbitalRadius.HasValue)
					body.OrbitalRadius = new Vector(body.RelativePosition.X, body.RelativePosition.Y).Length;
				if (!body.AngularVelocity.HasValue)
					body.AngularVelocity = Math.Sqrt(Math.Pow(body.OrbitalRadius.Value, 3.0) / body.Mu.Value);

				var currentAngle = MathUtility.DegreesToRadians(Vector.AngleBetween(new Vector(1, 0), new Vector(body.RelativePosition.X, body.RelativePosition.Y)));
				var newAngle = currentAngle + body.AngularVelocity.Value * Constants.SecondsPerTick;

				body.RelativePosition = new Point(body.OrbitalRadius.Value * Math.Cos(newAngle), body.OrbitalRadius.Value * Math.Sin(newAngle));
			}
		}

		const double c_gravitationalConstant = 6.67408E-11;
	}
}