public class ResetPasswordDto
{
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }
    public string NewPassword { get; set; } = null!;
}