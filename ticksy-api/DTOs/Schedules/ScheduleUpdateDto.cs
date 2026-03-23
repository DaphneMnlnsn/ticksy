using ticksy_api.Models;

public class ScheduleUpdateDto
{
    public required string ScheduleName { get; set; }
    public WorkSchedule.WorkArrangementType WorkArrangement { get; set; }
    public List<ScheduleDayDto> Days { get; set; } = [];
}