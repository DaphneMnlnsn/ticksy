using ticksy_api.Models;

public class ScheduleDetailsDto
{
    public int Id { get; set; }
    public string ScheduleName { get; set; } = null!;
    public WorkSchedule.WorkArrangementType WorkArrangement { get; set; }

    public int CreatedBy { get; set; }

    public List<ScheduleDayDto> Days { get; set; } = [];
    public List<UserWorkScheduleDto> UserWorkSchedules { get; set; } = [];

}