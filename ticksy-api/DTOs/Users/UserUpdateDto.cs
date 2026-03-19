using ticksy_api.Models;

public class UserUpdateDto
{
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    public string? TimeZone { get; set; }

    public User.UserRole? Role { get; set; }
}