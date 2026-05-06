using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticksy_api.Data;
using ticksy_api.Models;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public NotificationsController(AppDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateNotification([FromBody] NotificationCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var notif = new Notification
        {
            UserId =  dto.UserId,
            Type = dto.Type,
            Message = dto.Message,
            CreatedAt = DateTime.UtcNow,
        };

        _context.Notifications.Add(notif);

        await _context.SaveChangesAsync();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == dto.UserId && u.DeletedAt == null);

        if (user != null)
        {
            var body = $@"
                <h3>New Notification</h3>
                <p>Hello {user.FirstName},</p>
                <p>{dto.Message}</p>
                <br/>
                <small>This is an automated message.</small>
            ";

            if (dto.SendEmail && user != null)
            {
                await _emailService.SendEmailAsync(
                    user.Email,
                    "New Notification",
                    body
                );
            }
        }

        return Ok(new { message = "Notification created successfully." });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var data = await _context.Notifications
            .Include(n => n.User)
            .Select(n => new
            {
                n.Id,
                n.UserId,
                n.User.FirstName,
                n.User.MiddleName,
                n.User.LastName,
                n.Type,
                n.Message,
                n.ReadAt,
                n.CreatedAt
            })
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        
        var notifications = data.Select(n => new NotificationListDto
        {
            Id = n.Id,
            FullName = string.Join(" ", new[]
            {
                n.FirstName,
                n.MiddleName,
                n.LastName
            }.Where(x => !string.IsNullOrWhiteSpace(x))),
            Type = n.Type,
            Message = n.Message,
            ReadAt = n.ReadAt,
            CreatedAt = n.CreatedAt
        });

        return Ok(notifications);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetUserNotifications()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var data = await _context.Notifications
            .Where(u => u.UserId == userId)
            .Select(n => new
            {
                n.Id,
                n.UserId,
                n.User.FirstName,
                n.User.MiddleName,
                n.User.LastName,
                n.Type,
                n.Message,
                n.Status,
                n.ReadAt,
                n.CreatedAt
            })
            .ToListAsync();
        
        var notifications = data.Select(n => new NotificationListDto
        {
            Id = n.Id,
            FullName = string.Join(" ", new[]
            {
                n.FirstName,
                n.MiddleName,
                n.LastName
            }.Where(x => !string.IsNullOrWhiteSpace(x))),
            Type = n.Type,
            Message = n.Message,
            Status = n.Status,
            ReadAt = n.ReadAt,
            CreatedAt = n.CreatedAt
        });

        return Ok(notifications);
    }

    [Authorize]
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var notif = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (notif == null)
            return NotFound();

        notif.ReadAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Marked as read" });
    }
}