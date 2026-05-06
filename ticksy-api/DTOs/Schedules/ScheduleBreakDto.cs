public class ScheduleBreakDto
{
    public int ScheduleId { get; set; }
    public List<DayOfWeek> Days { get; set; } = [];
    public required string BreakName { get; set; }
    public TimeSpan BreakDuration { get; set; }
}