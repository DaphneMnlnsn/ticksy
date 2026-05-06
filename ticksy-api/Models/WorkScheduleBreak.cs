using ticksy_api.Models;

public class WorkScheduleBreak {
    public int Id { get; set; }

    public int WorkScheduleDayId { get; set; }
    public WorkScheduleDay WorkScheduleDay { get; set; } = null!;

    public required string BreakName { get; set; }
    public TimeSpan BreakDuration { get; set; }
}