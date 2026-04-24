public class UserWorkScheduleDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }

    public List<ScheduleDayDto> Schedule { get; set; } = [];
}