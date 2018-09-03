namespace OneSmallStep.Utility.Time
{
	public interface ICalendar
	{
		TimePoint CreateTimePoint(int year, int month, int day);
		string FormatTime(TimePoint point, TimeFormat format);
		string FormatOffset(TimeOffset offset, TimeFormat format);
		string FormatOffsetFrom(TimePoint point, TimeOffset offset, TimeFormat format);
	}
}
