public class TeamCreateDto
{
    public string TeamName { get; set; } = null!;
    public List<int> MemberIds { get; set; } = new();
}