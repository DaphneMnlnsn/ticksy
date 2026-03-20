using ticksy_api.Models;

public class TeamInvite
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public string Token { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; } = false;

    public DateTime CreatedAt { get; set; }
}