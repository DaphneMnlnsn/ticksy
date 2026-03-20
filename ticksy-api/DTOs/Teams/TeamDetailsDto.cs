public class TeamDetailsDto
{
    public int Id { get; set; }
    public string TeamName { get; set; } = null!;
    public int CreatedBy { get; set; }

    public List<TeamMemberDto> Members { get; set; } = [];
}