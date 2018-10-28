using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GoldenAnvil.Utility.Windows;
using OneSmallStep.Utility;
using OneSmallStep.Utility.Time;

namespace OneSmallStep.ECS.Components
{
	public sealed class EllipticalOrbitalPositionComponent : ComponentBase
	{
		public static EllipticalOrbitalPositionComponent CreatePoweredBody(Point startingPoint)
		{
			return new EllipticalOrbitalPositionComponent(startingPoint, null);
		}

		public static EllipticalOrbitalPositionComponent CreateUnpoweredBody()
		{
			return new EllipticalOrbitalPositionComponent(new Point(), null);
		}

		public static EllipticalOrbitalPositionComponent CreateUnpoweredBody(EntityId parentId, double semiMajorAxis, double eccentricity, double longitutdeOfPeriapsis, double meanAnomaly, bool isRetrograde)
		{
			var orbitalData = new OrbitalData
			{
				ParentId = parentId,
				SemiMajorAxis = semiMajorAxis,
				Eccentricity = eccentricity,
				LongitudeOfPeriapsis = longitutdeOfPeriapsis,
				MeanAnomalyAtTimeZero = meanAnomaly,
				IsRetrograde = isRetrograde,
			};
			return new EllipticalOrbitalPositionComponent(new Point(), orbitalData);
		}

		public Point RelativePosition
		{
			get => m_relativePosition;
			set => SetPropertyField(value, ref m_relativePosition);
		}

		public EntityId? ParentId => m_orbitalData?.ParentId;

		public double? SemiMajorAxis => m_orbitalData?.SemiMajorAxis;

		public double? SemiMinorAxis => m_orbitalData?.SemiMinorAxis;

		public double? Focus => m_orbitalData?.Focus;

		public double? Eccentricity => m_orbitalData?.Eccentricity;

		public double? LongitudeOfPeriapsis => m_orbitalData?.LongitudeOfPeriapsis;

		public Point GetCurrentAbsolutePosition(IEntityLookup entityLookup)
		{
			var position = m_relativePosition;
			var parentAbsolutePosition = GetParentEntity(entityLookup)?.GetOptionalComponent<EllipticalOrbitalPositionComponent>()?.GetCurrentAbsolutePosition(entityLookup);

			if (parentAbsolutePosition != null)
				position = position.WithOffset(parentAbsolutePosition.Value);
			return position;
		}

		public void SetCurrentAbsolutePosition(Point absolutePosition)
		{
			using (ScopedPropertyChange())
			{
				m_orbitalData = null;
				m_relativePosition = absolutePosition;
			}
		}

		public void InitializeOrbitalValuesIfNeeded(IEntityLookup entityLookup, double mass)
		{
			if (!m_isDirty)
				return;

			m_isDirty = false;
			if (m_orbitalData == null)
				return;

			var aa = m_orbitalData.SemiMajorAxis * m_orbitalData.SemiMajorAxis;

			if (aa < 1.0)
			{
				// sufficiently close to 0
				m_orbitalData.SemiMinorAxis = 0.0;
			}
			else
			{
				var ee = m_orbitalData.Eccentricity * m_orbitalData.Eccentricity;
				m_orbitalData.SemiMinorAxis = m_orbitalData.SemiMajorAxis * Math.Sqrt(1.0 - ee);
			}

			var bb = m_orbitalData.SemiMinorAxis * m_orbitalData.SemiMinorAxis;
			m_orbitalData.Focus = Math.Sqrt(aa - bb);

			var parentMass = GetParentEntity(entityLookup).GetRequiredComponent<OrbitalBodyCharacteristicsComponent>().Mass;
			var mu = parentMass * Constants.GravitationalConstant;
			m_orbitalData.Period = 2.0 * Math.PI * Math.Sqrt(aa * m_orbitalData.SemiMajorAxis / mu);
		}

		public Point GetRelativeOrbitalPositionAtTime(TimePoint timePoint)
		{
			if (m_orbitalData == null)
				return m_relativePosition;

			var timeInSeconds = timePoint.Tick * Constants.SecondsPerTick;
			var meanAnomaly = timeInSeconds % m_orbitalData.Period;
			if (m_orbitalData.IsRetrograde)
				meanAnomaly *= -1;
			meanAnomaly = 2.0 * Math.PI * meanAnomaly / m_orbitalData.Period;
			meanAnomaly += m_orbitalData.MeanAnomalyAtTimeZero * Math.PI / 180.0;
			var anomaly = CalculateAnomaly(meanAnomaly);

			var x = Math.Cos(anomaly) - m_orbitalData.Eccentricity;
			var y = Math.Sin(anomaly) * Math.Sqrt(Math.Abs(1.0 - m_orbitalData.Eccentricity * m_orbitalData.Eccentricity));

			var trueAnomaly = Math.Atan2(y, x);
			var r0 = (m_orbitalData.SemiMajorAxis * (1.0 - m_orbitalData.Eccentricity)) * (1.0 + m_orbitalData.Eccentricity);
			var r = r0 / (1.0 + m_orbitalData.Eccentricity * Math.Cos(trueAnomaly));

			var angle = trueAnomaly + (m_orbitalData.LongitudeOfPeriapsis * Math.PI / 180.0);
			x = r * Math.Cos(angle);
			y = r * Math.Sin(angle);

			return new Point(x, y);
		}

		public Point GetAbsoluteOrbitalPositionAtTime(IEntityLookup entityLookup, TimePoint timePoint)
		{
			var parentPosition = GetParentEntity(entityLookup)?.GetOptionalComponent<EllipticalOrbitalPositionComponent>()?.GetAbsoluteOrbitalPositionAtTime(entityLookup, timePoint);
			if (parentPosition == null)
				return m_relativePosition;
			return GetRelativeOrbitalPositionAtTime(timePoint).WithOffset(parentPosition.Value);
		}

		public IEnumerable<EllipticalOrbitalPositionComponent> EnumerateThisAndParents(IEntityLookup entityLookup)
		{
			var parent = GetParentEntity(entityLookup)?.GetOptionalComponent<EllipticalOrbitalPositionComponent>();
			if (parent == null)
				return Enumerable.Repeat(this, 1);
			return parent.EnumerateThisAndParents(entityLookup).Prepend(this);
		}

		public override ComponentBase Clone()
		{
			return new EllipticalOrbitalPositionComponent(this);
		}

		private EllipticalOrbitalPositionComponent(Point relativePosition, OrbitalData orbitalData)
		{
			m_relativePosition = relativePosition;
			m_orbitalData = orbitalData;
			m_isDirty = true;
		}

		private EllipticalOrbitalPositionComponent(EllipticalOrbitalPositionComponent that)
		: base(that)
		{
			m_relativePosition = that.m_relativePosition;
			m_orbitalData = that.m_orbitalData?.Clone();
			m_isDirty = that.m_isDirty;
		}

		private Entity GetParentEntity(IEntityLookup entityLookup)
		{
			return m_orbitalData != null ? entityLookup.GetEntity(m_orbitalData.ParentId) : null;
		}

		private double CalculateAnomaly(double meanAnomaly)
		{
			if (Math.Abs(meanAnomaly) < 1e-20)
				return 0.0;

			double current = 0;
			double error = 0;
			double offset = 0;
			bool isNegative = false;
			double deltaCurrent = 1;

			var eccentricityDelta = Math.Abs(1.0 - m_orbitalData.Eccentricity);
			double threshold = Math.Max(c_threshold * eccentricityDelta, c_minThreshold);

			// low-eccentricity formula from Meeus, p. 195
			if (m_orbitalData.Eccentricity < 0.3)
			{
				current = Math.Atan2(Math.Sin(meanAnomaly), Math.Cos(meanAnomaly) - m_orbitalData.Eccentricity);
				while (Math.Abs(deltaCurrent) > threshold)
				{
					error = current - m_orbitalData.Eccentricity * Math.Sin(current) - meanAnomaly;
					deltaCurrent = -error / (1.0 - m_orbitalData.Eccentricity * Math.Cos(current));
					current += deltaCurrent;
				}
				return current;
			}

			if (m_orbitalData.Eccentricity < 1.0)
			{
				if (meanAnomaly < -Math.PI || meanAnomaly > Math.PI)
				{
					// adjust mean anomaly to within -PI to PI
					double tMod = meanAnomaly % (Math.PI * 2.0);
					if (tMod > Math.PI)
						tMod -= 2.0 * Math.PI;
					else if (tMod < -Math.PI)
						tMod += 2.0 * Math.PI;
					offset = meanAnomaly - tMod;
					meanAnomaly = tMod;
				}
			}

			if (meanAnomaly < 0)
			{
				meanAnomaly = -meanAnomaly;
				isNegative = true;
			}

			current = meanAnomaly;

			// up to 60 degrees
			if (((m_orbitalData.Eccentricity > 0.8) && (meanAnomaly < Math.PI / 3.0)) || (m_orbitalData.Eccentricity > 1.0))
			{
				double trial = meanAnomaly / eccentricityDelta;

				// cubic term is dominant
				if (trial * trial > 6.0 * eccentricityDelta)
				{
					if (meanAnomaly < Math.PI)
					{
						trial = FastCubeRoot(6.0 * meanAnomaly);
					}
					else
					{
						// hyperbolic with 5th and higher-order terms predominant
						trial = Asinh(meanAnomaly / m_orbitalData.Eccentricity);
					}
				}

				current = trial;
			}

			// hyperbolic, large-mean-anomaly case
			if ((m_orbitalData.Eccentricity > 1.0) && (meanAnomaly > 4.0))
				current = Math.Log(meanAnomaly);

			if (m_orbitalData.Eccentricity < 1.0)
			{
				int i = 0;
				while (Math.Abs(deltaCurrent) > threshold)
				{
					if (i++ > c_maxIterations)
						error = CalculateErrorForNearlyParabolic(current) - meanAnomaly;
					else
						error = current - m_orbitalData.Eccentricity * Math.Sin(current) - meanAnomaly;
					deltaCurrent = -error / (1.0 - m_orbitalData.Eccentricity * Math.Cos(current));
					current += deltaCurrent;
				}
			}
			else
			{
				int i = 0;
				while (Math.Abs(deltaCurrent) > threshold)
				{
					if (i++ > c_maxIterations)
						error = -CalculateErrorForNearlyParabolic(current) - meanAnomaly;
					else
						error = m_orbitalData.Eccentricity * Math.Sinh(current) - current - meanAnomaly;
					deltaCurrent = -error / (m_orbitalData.Eccentricity * Math.Cosh(current) - 1.0);
					current += deltaCurrent;
				}
			}

			return isNegative ? offset - current : offset + current;
		}

		private double CalculateErrorForNearlyParabolic(double eccAnomaly)
		{
			double anomaly2 = (m_orbitalData.Eccentricity > 1.0 ? eccAnomaly * eccAnomaly : -eccAnomaly * eccAnomaly);
			double term = m_orbitalData.Eccentricity * anomaly2 * eccAnomaly / 6.0;
			double rVal = (1.0 - m_orbitalData.Eccentricity) * eccAnomaly - term;
			int n = 4;
			while (Math.Abs(term) > 1e-15)
			{
				term *= anomaly2 / (double) (n * (n + 1));
				rVal -= term;
				n += 2;
			}

			return rVal;
		}

		private static double FastCubeRoot(double x)
		{
			return Math.Exp(Math.Log(x) / 3.0);
		}

		private static double Asinh(double x)
		{
			if (x > 0)
				return Math.Log(x + Math.Sqrt(x * x + 1));
			return -Math.Log(-x + Math.Sqrt(x * x + 1));
		}

		private sealed class OrbitalData
		{
			public EntityId ParentId { get; set; }
			public double SemiMajorAxis { get; set; }
			public double SemiMinorAxis { get; set; }
			public double Eccentricity { get; set; }
			public double LongitudeOfPeriapsis { get; set; }
			public double MeanAnomaly { get; set; }
			public double MeanAnomalyAtTimeZero { get; set; }
			public double Period { get; set; }
			public double Focus { get; set; }
			public bool IsRetrograde { get; set; }

			public OrbitalData Clone()
			{
				return new OrbitalData
				{
					ParentId = ParentId,
					SemiMajorAxis = SemiMajorAxis,
					SemiMinorAxis = SemiMinorAxis,
					Eccentricity = Eccentricity,
					LongitudeOfPeriapsis = LongitudeOfPeriapsis,
					MeanAnomaly = MeanAnomaly,
					MeanAnomalyAtTimeZero = MeanAnomalyAtTimeZero,
					Period = Period,
					Focus = Focus,
					IsRetrograde = IsRetrograde,
				};
			}
		}

		const double c_threshold = 1.0e-8;
		const double c_minThreshold = 1.0e-15;
		const int c_maxIterations = 7;

		OrbitalData m_orbitalData;
		Point m_relativePosition;
		bool m_isDirty;
	}
}