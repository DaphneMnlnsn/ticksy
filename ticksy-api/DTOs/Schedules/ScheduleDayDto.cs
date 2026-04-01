public class ScheduleDayDto
{
    public DayOfWeek Day { get; set; }

    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }

    public bool IsRestDay { get; set; }
    public List<ScheduleBreakDisplayDto> Breaks { get; set; } = [];
}