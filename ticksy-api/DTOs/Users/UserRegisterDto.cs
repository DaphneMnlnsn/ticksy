public class UserRegisterDto
{
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password{ get; set; }
    public required string Phone { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    public required string TimeZone { get; set; } = "Asia/Manila";
    public bool AutoClockIn { get; set; } =  false;
}