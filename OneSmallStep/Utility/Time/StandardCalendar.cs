using System;
using GoldenAnvil.Utility;
using static System.Math;

namespace OneSmallStep.Utility.Time
{
	public sealed class StandardCalendar : ICalendar
	{
		public static TimeOffset CreateTimeOffsetInYears(int years, double ticksPerDay)
		{
			return new TimeOffset((long) (ticksPerDay * GetDaysInYears(years)));
		}

		public TimePoint CreateTimePoint(int year, int month, int day)
		{
			return new TimePoint(CreateTimePoint(year, month, day, m_ticksPerDay).Tick - m_startDay);
		}

		public static StandardCalendar Create(int startYear, int startMonth, int startDay, double ticksPerDay)
		{
			var startPoint = CreateTimePoint(startYear, startMonth, startDay, ticksPerDay);

			return new StandardCalendar((long) (startPoint.Tick / ticksPerDay), ticksPerDay);
		}

		public string FormatTime(TimePoint point, TimeFormat format)
		{
			DaysToDaysMonthsYears((long) Floor(point.Tick / m_ticksPerDay) + m_startDay, out var years, out var months,
				out var days);
			var date = new DateTime(years, months, days);
			return (format == TimeFormat.Long ? "{0:D}" : "{0:d}").FormatCurrentCulture(date);
		}

		public string FormatOffset(TimeOffset offset, TimeFormat format)
		{
			var days = (long) Floor(offset.TickOffset / m_ticksPerDay);
			return OurResources.ResourceManager.Pluralize(format == TimeFormat.Long ? "TimeOffsetDayLong" : "TimeOffsetDayShort", days).FormatCurrentCulture(days);
		}

		public string FormatOffsetFrom(TimePoint point, TimeOffset offset, TimeFormat format)
		{
			if (offset.TickOffset < 0)
				throw new ArgumentException($"{nameof(offset)} must be positive");

			var startTotalDays = (long) Floor(point.Tick / m_ticksPerDay) + m_startDay;
			DaysToDaysMonthsYears(startTotalDays, out var startYears, out var startMonths,
				out var startDays);

			var endTotalDays = startTotalDays + offset.TickOffset;
			DaysToDaysMonthsYears(endTotalDays, out var endYears, out var endMonths,
				out var endDays);

			var days = endDays >= startDays ? endDays - startDays : GetDaysInMonth(startMonths, startYears) - startDays + endDays;
			var months = endMonths >= startMonths ? endMonths - startMonths : 12 - startMonths + endMonths;
			if (endDays < startDays)
				months--;
			var years = endYears - startYears;
			if (endMonths < startMonths)
				years--;

			var output = years != 0 ? OurResources.ResourceManager.Pluralize(format == TimeFormat.Long ? "TimeOffsetYearLong" : "TimeOffsetYearShort", years).FormatCurrentCulture(years) : "";
			var monthsOutput = months != 0 ? OurResources.ResourceManager.Pluralize(format == TimeFormat.Long ? "TimeOffsetMonthLong" : "TimeOffsetMonthShort", months).FormatCurrentCulture(months) : null;
			if (monthsOutput != null)
			{
				if (output.Length != 0)
					output = OurResources.TimeOffsetTermJoin.FormatCurrentCulture(output, monthsOutput);
				else
					output = monthsOutput;
			}
			var daysOutput = days != 0 ? OurResources.ResourceManager.Pluralize(format == TimeFormat.Long ? "TimeOffsetDayLong" : "TimeOffsetDayShort", days).FormatCurrentCulture(days) : null;
			if (daysOutput != null)
			{
				if (output.Length != 0)
					output = OurResources.TimeOffsetTermJoin.FormatCurrentCulture(output, daysOutput);
				else
					output = daysOutput;
			}

			return output;
		}

		private StandardCalendar(long startDay, double ticksPerDay)
		{
			m_startDay = startDay;
			m_ticksPerDay = ticksPerDay;
		}

		private static TimePoint CreateTimePoint(int years, int months, int days, double ticksPerDay)
		{
			if (months < 1 || months > 12)
				throw new ArgumentException($"{nameof(months)} must be between 1 and 12.");
			if (days < 1 || days > 31)
				throw new ArgumentException($"{nameof(days)} must be between 1 and 31.");

			var totalDays = GetDaysInYears(years);
			totalDays += s_daysToStartOfMonth[months - 1];
			if (IsLeapYear(years) && months > 2)
				totalDays++;

			totalDays += days;

			return new TimePoint((long) (totalDays * ticksPerDay));
		}

		private static long GetDaysInYears(int years)
		{
			var totalDays = (years / 400) * c_daysIn400Years;
			var remainingYears = years % 400;
			totalDays += (remainingYears / 100) * c_daysIn100Years;
			remainingYears = remainingYears % 100;
			totalDays += (remainingYears / 4) * c_daysIn4Years;
			remainingYears = remainingYears % 4;
			if (remainingYears != 0 && IsLeapYear(years - remainingYears))
			{
				totalDays += 366;
				remainingYears--;
			}
			totalDays += remainingYears * 365;
			return totalDays;
		}

		private static void DaysToDaysMonthsYears(long totalDays, out int years, out int months, out int days)
		{
			var localDays = totalDays;
			years = (int) ((localDays / c_daysIn400Years) * 400);
			localDays = localDays % c_daysIn400Years;
			years += (int) ((localDays / c_daysIn100Years) * 100);
			localDays = localDays % c_daysIn100Years;
			years += (int) ((localDays / c_daysIn4Years) * 4);
			localDays = localDays % c_daysIn4Years;
			var isLeapYear = IsLeapYear(years);
			if (localDays > (isLeapYear ? 366 : 365))
			{
				years++;
				localDays -= isLeapYear ? 366 : 365;

				years += (int) (localDays / 365);
				localDays = localDays % 365;
			}
			if (localDays == 0)
			{
				localDays = 365;
				years--;
			}

			isLeapYear = IsLeapYear(years);
			months = GetMonthFromDayOfYear((int) localDays, isLeapYear);
			localDays -= s_daysToStartOfMonth[months - 1];
			if (isLeapYear && months > 2)
				localDays--;
			days = (int) localDays;
		}

		private static bool IsLeapYear(long year)
		{
			if (year % 400 == 0)
				return true;
			if ((year % 4 == 0) && (year % 100 != 0))
				return true;
			return false;
		}

		private static int GetMonthFromDayOfYear(int dayOfYear, bool isLeapYear)
		{
			if (isLeapYear)
			{
				if (dayOfYear <= 182)
				{
					if (dayOfYear <= 91)
					{
						if (dayOfYear <= 31)
							return 1;
						return (dayOfYear <= 60) ? 2 : 3;
					}
					if (dayOfYear <= 121)
						return 4;
					return (dayOfYear <= 152) ? 5 : 6;
				}
				if (dayOfYear <= 274)
				{
					if (dayOfYear <= 213)
						return 7;
					return (dayOfYear <= 244) ? 8 : 9;
				}
				if (dayOfYear <= 305)
					return 10;
				return (dayOfYear <= 335) ? 11 : 12;
			}

			if (dayOfYear <= 181)
			{
				if (dayOfYear <= 90)
				{
					if (dayOfYear <= 31)
						return 1;
					return (dayOfYear <= 59) ? 2 : 3;
				}
				if (dayOfYear <= 120)
					return 4;
				return (dayOfYear <= 151) ? 5 : 6;
			}
			if (dayOfYear <= 273)
			{
				if (dayOfYear <= 212)
					return 7;
				return (dayOfYear <= 243) ? 8 : 9;
			}
			if (dayOfYear <= 304)
				return 10;
			return (dayOfYear <= 334) ? 11 : 12;
		}

		private int GetDaysInMonth(int month, int year)
		{
			int yearToCheck = year;
			int index = month - 1;
			if (month < 0)
			{
				index = 11;
				yearToCheck--;
			}

			var days = s_daysInMonth[index];
			if (index == 1 && IsLeapYear(yearToCheck))
				days++;

			return days;
		}

		const long c_daysIn400Years = (400 * 365) + 97;
		const long c_daysIn100Years = (100 * 365) + 24;
		const long c_daysIn4Years = (4 * 365) + 1;

		private static readonly long[] s_daysToStartOfMonth = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334 };
		private static readonly int[] s_daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

		readonly long m_startDay;
		readonly double m_ticksPerDay;
	}
}
