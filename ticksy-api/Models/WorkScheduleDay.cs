using ticksy_api.Models;

public class WorkScheduleDay {
    public int Id { get; set; }

    public int WorkScheduleId { get; set; }
    public WorkSchedule WorkSchedule { get; set; } = null!;

    public DayOfWeek Day { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public bool IsRestDay { get; set; } = false;

    public List<WorkScheduleBreak> Breaks { get; set; } = [];
}