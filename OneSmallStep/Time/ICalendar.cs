namespace OneSmallStep.Time
{
	public enum TimeFormat
	{
		Short,
		Long,
	}
	public interface ICalendar
	{
		string FormatTime(TimePoint point, TimeFormat format);
		string FormatOffsetFrom(TimePoint point, TimeOffset offset, TimeFormat format);
	}
}
