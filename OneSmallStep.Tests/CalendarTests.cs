using System.Globalization;
using NUnit.Framework;
using OneSmallStep.Time;

namespace OneSmallStep.Tests
{
	[TestFixture]
	public class CalendarTests
	{
		[OneTimeSetUp]
		public void Setup()
		{
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");
		}

		[TestCase(2000, 1, 1, TimeFormat.Short, "1/1/2000")]
		[TestCase(2001, 1, 1, TimeFormat.Short, "1/1/2001")]
		[TestCase(2004, 1, 1, TimeFormat.Short, "1/1/2004")]
		[TestCase(2100, 1, 1, TimeFormat.Short, "1/1/2100")]
		[TestCase(2400, 1, 1, TimeFormat.Short, "1/1/2400")]
		[TestCase(2001, 4, 1, TimeFormat.Short, "4/1/2001")]
		[TestCase(2001, 5, 1, TimeFormat.Short, "5/1/2001")]
		[TestCase(2001, 6, 1, TimeFormat.Short, "6/1/2001")]
		[TestCase(2001, 7, 1, TimeFormat.Short, "7/1/2001")]
		[TestCase(2001, 8, 1, TimeFormat.Short, "8/1/2001")]
		[TestCase(2001, 9, 1, TimeFormat.Short, "9/1/2001")]
		[TestCase(2001, 10, 1, TimeFormat.Short, "10/1/2001")]
		[TestCase(2001, 11, 1, TimeFormat.Short, "11/1/2001")]
		[TestCase(2001, 12, 1, TimeFormat.Short, "12/1/2001")]
		[TestCase(2001, 1, 31, TimeFormat.Short, "1/31/2001")]
		[TestCase(2001, 2, 1, TimeFormat.Short, "2/1/2001")]
		[TestCase(2001, 3, 31, TimeFormat.Short, "3/31/2001")]
		[TestCase(2001, 4, 30, TimeFormat.Short, "4/30/2001")]
		[TestCase(2001, 5, 31, TimeFormat.Short, "5/31/2001")]
		[TestCase(2001, 6, 30, TimeFormat.Short, "6/30/2001")]
		[TestCase(2001, 7, 31, TimeFormat.Short, "7/31/2001")]
		[TestCase(2001, 8, 31, TimeFormat.Short, "8/31/2001")]
		[TestCase(2001, 9, 30, TimeFormat.Short, "9/30/2001")]
		[TestCase(2001, 10, 31, TimeFormat.Short, "10/31/2001")]
		[TestCase(2001, 11, 30, TimeFormat.Short, "11/30/2001")]
		[TestCase(2000, 2, 29, TimeFormat.Short, "2/29/2000")]
		[TestCase(2001, 2, 28, TimeFormat.Short, "2/28/2001")]
		[TestCase(2004, 2, 29, TimeFormat.Short, "2/29/2004")]
		[TestCase(2100, 2, 28, TimeFormat.Short, "2/28/2100")]
		[TestCase(2400, 2, 29, TimeFormat.Short, "2/29/2400")]
		[TestCase(2000, 3, 1, TimeFormat.Short, "3/1/2000")]
		[TestCase(2001, 3, 1, TimeFormat.Short, "3/1/2001")]
		[TestCase(2004, 3, 1, TimeFormat.Short, "3/1/2004")]
		[TestCase(2100, 3, 1, TimeFormat.Short, "3/1/2100")]
		[TestCase(2400, 3, 1, TimeFormat.Short, "3/1/2400")]
		[TestCase(2000, 12, 31, TimeFormat.Short, "12/31/2000")]
		[TestCase(2001, 12, 31, TimeFormat.Short, "12/31/2001")]
		[TestCase(2004, 12, 31, TimeFormat.Short, "12/31/2004")]
		[TestCase(2100, 1, 1, TimeFormat.Short, "1/1/2100")]
		[TestCase(2100, 12, 31, TimeFormat.Short, "12/31/2100")]
		[TestCase(2101, 1, 1, TimeFormat.Short, "1/1/2101")]
		[TestCase(2400, 12, 31, TimeFormat.Short, "12/31/2400")]
		public void TestPointFormat(int year, int month, int day, TimeFormat format, string expectedOutput)
		{
			var calendar = StandardCalendar.Create(year, month, day, 1);
			Assert.AreEqual(expectedOutput, calendar.FormatTime(new TimePoint(), format));
		}

		[TestCase(2000, 1, 1, -1, TimeFormat.Short, "12/31/1999")]
		[TestCase(1999, 12, 31, 1, TimeFormat.Short, "1/1/2000")]
		[TestCase(2000, 2, 29, -1, TimeFormat.Short, "2/28/2000")]
		[TestCase(2000, 2, 29, 1, TimeFormat.Short, "3/1/2000")]
		[TestCase(2000, 3, 1, -1, TimeFormat.Short, "2/29/2000")]
		[TestCase(2000, 2, 28, 1, TimeFormat.Short, "2/29/2000")]
		[TestCase(2001, 2, 28, 1, TimeFormat.Short, "3/1/2001")]
		[TestCase(2001, 3, 1, -1, TimeFormat.Short, "2/28/2001")]
		[TestCase(2000, 1, 1, 365, TimeFormat.Short, "12/31/2000")]
		[TestCase(2000, 1, 1, 366, TimeFormat.Short, "1/1/2001")]
		[TestCase(2001, 1, 1, 364, TimeFormat.Short, "12/31/2001")]
		[TestCase(2001, 1, 1, 365, TimeFormat.Short, "1/1/2002")]
		[TestCase(2100, 1, 1, 364, TimeFormat.Short, "12/31/2100")]
		[TestCase(2100, 1, 1, 365, TimeFormat.Short, "1/1/2101")]
		public void TestPointFormatWithOffset(int year, int month, int day, long offset, TimeFormat format,
			string expectedOutput)
		{
			var calendar = StandardCalendar.Create(year, month, day, 1);
			Assert.AreEqual(expectedOutput, calendar.FormatTime(new TimePoint(offset), format));
		}

		[TestCase(0, TimeFormat.Long, "0 days")]
		[TestCase(1, TimeFormat.Long, "1 day")]
		[TestCase(2, TimeFormat.Long, "2 days")]
		[TestCase(0, TimeFormat.Short, "0d")]
		[TestCase(1, TimeFormat.Short, "1d")]
		[TestCase(2, TimeFormat.Short, "2d")]
		public void TestOffsetFormat(long offset, TimeFormat format, string expectedOutput)
		{
			var calendar = StandardCalendar.Create(2000, 1, 1, 1);
			Assert.AreEqual(expectedOutput, calendar.FormatOffset(new TimeOffset(offset), format));
		}

		[TestCase(2000, 1, 1, 1, TimeFormat.Long, "1 day")]
		[TestCase(2000, 1, 31, 1, TimeFormat.Long, "1 day")]
		[TestCase(2000, 2, 28, 1, TimeFormat.Long, "1 day")]
		[TestCase(2000, 2, 29, 1, TimeFormat.Long, "1 day")]
		[TestCase(2000, 12, 31, 1, TimeFormat.Long, "1 day")]
		[TestCase(2001, 1, 1, 1, TimeFormat.Long, "1 day")]
		[TestCase(2001, 1, 31, 1, TimeFormat.Long, "1 day")]
		[TestCase(2001, 2, 27, 1, TimeFormat.Long, "1 day")]
		[TestCase(2001, 2, 28, 1, TimeFormat.Long, "1 day")]
		[TestCase(2001, 12, 31, 1, TimeFormat.Long, "1 day")]
		[TestCase(2000, 1, 1, 30, TimeFormat.Long, "30 days")]
		[TestCase(2000, 1, 1, 31, TimeFormat.Long, "1 month")]
		[TestCase(2000, 1, 1, 32, TimeFormat.Long, "1 month, 1 day")]
		[TestCase(2000, 1, 1, 31, TimeFormat.Long, "1 month")]
		[TestCase(2000, 1, 31, 32, TimeFormat.Long, "1 month, 3 days")]
		[TestCase(2000, 1, 31, 29, TimeFormat.Long, "29 days")]
		[TestCase(2000, 2, 28, 32, TimeFormat.Long, "1 month, 3 days")]
		[TestCase(2000, 2, 28, 28, TimeFormat.Long, "28 days")]
		[TestCase(2000, 2, 28, 29, TimeFormat.Long, "1 month")]
		[TestCase(2000, 2, 29, 32, TimeFormat.Long, "1 month, 1 day")]
		[TestCase(2000, 2, 29, 27, TimeFormat.Long, "27 days")]
		[TestCase(2000, 12, 31, 32, TimeFormat.Long, "1 month, 1 day")]
		[TestCase(2000, 12, 31, 31, TimeFormat.Long, "1 month")]
		[TestCase(2001, 1, 1, 31, TimeFormat.Long, "1 month")]
		[TestCase(2001, 1, 1, 32, TimeFormat.Long, "1 month, 1 day")]
		[TestCase(2001, 1, 1, 31, TimeFormat.Long, "1 month")]
		[TestCase(2001, 1, 31, 32, TimeFormat.Long, "1 month, 4 days")]
		[TestCase(2001, 1, 31, 29, TimeFormat.Long, "1 month, 1 day")]
		[TestCase(2001, 2, 27, 32, TimeFormat.Long, "1 month, 4 days")]
		[TestCase(2001, 2, 27, 28, TimeFormat.Long, "1 month")]
		[TestCase(2001, 2, 28, 32, TimeFormat.Long, "1 month, 1 day")]
		[TestCase(2001, 2, 28, 27, TimeFormat.Long, "27 days")]
		[TestCase(2001, 12, 31, 32, TimeFormat.Long, "1 month, 1 day")]
		[TestCase(2001, 12, 31, 31, TimeFormat.Long, "1 month")]
		public void TestOffsetFromFormat(int year, int month, int day, long offset, TimeFormat format, string expectedOutput)
		{
			var calendar = StandardCalendar.Create(year, month, day, 1);
			Assert.AreEqual(expectedOutput, calendar.FormatOffsetFrom(new TimePoint(), new TimeOffset(offset), format));
		}
	}
}
