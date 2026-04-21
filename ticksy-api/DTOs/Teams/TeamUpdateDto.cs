public class TeamUpdateDto
{
    public required string TeamName { get; set; }
    public List<int> MemberIds { get; set; } = [];
}