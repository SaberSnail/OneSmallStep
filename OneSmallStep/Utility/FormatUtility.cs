using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldenAnvil.Utility;

namespace OneSmallStep.Utility
{
	public static class FormatUtility
	{
		public static string RenderDistance(double distance)
		{
			if (distance < 1E3)
				return OurResources.DIstanceInMeters.FormatCurrentCulture(distance);
			if (distance < 1E6)
				return OurResources.DistanceInKilometers.FormatCurrentCulture(distance / 1E3);
			if (distance < 1E9)
				return OurResources.DistanceInMegameters.FormatCurrentCulture(distance / 1E6);
			if (distance < 1E12)
				return OurResources.DistanceInGigameters.FormatCurrentCulture(distance / 1E9);
			if (distance < 1E15)
				return OurResources.DistanceInTerameters.FormatCurrentCulture(distance / 1E12);
			if (distance < 1E18)
				return OurResources.DistanceInPetameters.FormatCurrentCulture(distance / 1E15);
			if (distance < 1E21)
				return OurResources.DistanceInExameters.FormatCurrentCulture(distance / 1E18);
			return OurResources.DIstanceInMeters.FormatCurrentCulture(distance);
		}
	}
}
