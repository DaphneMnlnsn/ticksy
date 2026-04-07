using ticksy_api.Models;

public class ScheduleCreateDto
{
    public required string ScheduleName { get; set; }
    public WorkSchedule.WorkArrangementType WorkArrangement { get; set; }
    public string? WeeklyDuration { get; set; }
    public List<ScheduleDayDto>? Days { get; set; } = [];
}