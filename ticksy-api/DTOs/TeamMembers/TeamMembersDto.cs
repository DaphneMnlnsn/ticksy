public class TeamMemberDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime JoinedAt { get; set; }

    public string Email { get; set; } = null!;
}