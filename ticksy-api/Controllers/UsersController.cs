using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ticksy_api.Data;
using ticksy_api.Models;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _config;
    private readonly EmailService _emailService;

    public UsersController(AppDbContext context, IConfiguration config, EmailService emailService)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
        _config = config;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.DeletedAt == null))
            return BadRequest("Email is already in use.");
        
        var user = new User
        {
            FirstName =  dto.FirstName,
            MiddleName = dto.MiddleName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            AvatarUrl = dto.AvatarUrl,
            TimeZone = dto.TimeZone,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.DeletedAt == null);
        if (user == null)
            return Unauthorized();
        
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result != PasswordVerificationResult.Success)
            return Unauthorized("Invalid email and password");

        if (user.AutoClockIn)
        {
            await AutoClockIn(user.Id);
        }
        
        var token = GenerateJwtToken(user);

        return Ok(new { token, userId = user.Id, role = user.Role.ToString() });
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.ToString()),
            new Claim("role", user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
            .Where(u => u.DeletedAt == null)
            .Select(u => new
            {
                u.Id,
                u.FirstName,
                u.MiddleName,
                u.LastName,
                u.Email,
                u.Phone,
                u.TimeZone,
                u.Address,
                u.AvatarUrl,
                Role = u.Role.ToString(),
                u.LastActive,
                u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound();

        if (user.DeletedAt != null)
            return NotFound("This user account has been deleted.");

        var result = new
        {
            user.Id,
            user.FirstName,
            user.MiddleName,
            user.LastName,
            user.Email,
            user.Phone,
            user.TimeZone,
            user.Address,
            user.AvatarUrl,
            Role = user.Role.ToString(),
            user.LastActive,
            user.CreatedAt
        };

        return Ok(result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        if (user.DeletedAt != null)
            return BadRequest("Cannot update a deleted user.");

        if(!string.IsNullOrWhiteSpace(dto.FirstName)) user.FirstName = dto.FirstName;
        if(!string.IsNullOrWhiteSpace(dto.MiddleName)) user.MiddleName = dto.MiddleName;
        if(!string.IsNullOrWhiteSpace(dto.LastName)) user.LastName = dto.LastName;
        if(!string.IsNullOrWhiteSpace(dto.Phone)) user.Phone = dto.Phone;
        if(!string.IsNullOrWhiteSpace(dto.Address)) user.Address = dto.Address;
        if(!string.IsNullOrWhiteSpace(dto.AvatarUrl)) user.AvatarUrl = dto.AvatarUrl;
        if (dto.Role.HasValue) user.Role = dto.Role.Value;
        if(!string.IsNullOrWhiteSpace(dto.TimeZone)) user.TimeZone = dto.TimeZone;

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "User updated successfully."});
    }

    [Authorize]
    [HttpPut("change-pass")]
    public async Task<IActionResult> ChangeUserPass(int id, [FromBody] UserChangePassDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null)
            return Unauthorized();

        int userId = int.Parse(userIdClaim.Value);

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound();

        if (user.DeletedAt != null)
            return BadRequest("User account is deleted.");

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.CurrentPassword
        );

        if (result != PasswordVerificationResult.Success)
            return BadRequest("Current password is incorrect.");

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Password changed successfully." });
    }

    [HttpPut("forgot-pass")] //PENDING TEST
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email && u.DeletedAt == null);

        if (user == null)
            return Ok(new { message = "If the email exists, a reset link has been sent." });

        var token = Guid.NewGuid().ToString();

        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

        await _context.SaveChangesAsync();

        var resetLink = $"http://localhost:3000/reset-password?token={token}";

        var body = $@"
            <h3>Password Reset</h3>
            <p>Click the link below to reset your password:</p>
            <a href='{resetLink}'>Reset Password</a>
            <p>This link expires in 15 minutes.</p>
        ";

        await _emailService.SendEmailAsync(user.Email, "Reset Password", body);

        return Ok(new { message = "Reset link sent." });
    }

    [HttpPut("reset-pass")] //PENDING TEST
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        
        var user = await _context.Users
            .FirstOrDefaultAsync(u => 
            u.PasswordResetToken == dto.PasswordResetToken && u.PasswordResetTokenExpiry > DateTime.UtcNow);

        if (user == null)
            return BadRequest("Invalid or expired token.");

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Password reset successful." });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        if (user.DeletedAt != null)
            return BadRequest("User is already deleted.");

        user.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "User deleted successfully."});
    }

    private async Task AutoClockIn(int userId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await _context.Users
            .Include(u => u.UserWorkSchedules)
                .ThenInclude(s => s.WorkSchedule)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var existing = await _context.Attendances
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Date == today);

        if (existing != null)
            return;

        var now = DateTime.UtcNow;
        var nowLocal = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, user.TimeZone);

        var attendance = new Attendance
        {
            UserId = userId,
            Date = DateOnly.FromDateTime(now),
            TimeIn = TimeOnly.FromDateTime(now),
            ClockInSource = Attendance.ClockInSourceEnum.AutoLogin,
            CreatedAt = now,
            UpdatedAt = now
        };

        var todayDayOfWeek = DateTime.UtcNow.DayOfWeek;

        var scheduleDay = user.UserWorkSchedules
            .SelectMany(us => us.WorkSchedule.ScheduleDays)
            .FirstOrDefault(sd => sd.Day == todayDayOfWeek);

        if (scheduleDay?.StartTime != null)
        {
            if (attendance.TimeIn > scheduleDay.StartTime.Value)
            {
                attendance.LateMinutes =
                    (int)(attendance.TimeIn - scheduleDay.StartTime.Value).TotalMinutes;
            }
        }

        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();
    }
}