public class TeamCreateDto
{
    public required string TeamName { get; set; }
    public List<int> MemberIds { get; set; } = new();
}