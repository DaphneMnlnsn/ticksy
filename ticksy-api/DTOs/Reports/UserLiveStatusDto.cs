public class UserLiveStatusDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string Status { get; set; } = "Out";
    public DateOnly? LastActionDate { get; set; }
    public TimeOnly? LastActionTime { get; set; }
}