namespace OneSmallStep.Time
{
	public interface ICalendar
	{
		string FormatTime(TimePoint point, TimeFormat format);
		string FormatOffset(TimeOffset offset, TimeFormat format);
		string FormatOffsetFrom(TimePoint point, TimeOffset offset, TimeFormat format);
	}
}
