using ticksy_api.Data;
using ticksy_api.Models;

public class NotificationService
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public NotificationService(AppDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task CreateAsync(int userId, Notification.NotifType type, string message, bool sendEmail = false)
    {
        var notif = new Notification
        {
            UserId = userId,
            Type = type,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notif);
        await _context.SaveChangesAsync();

        if (sendEmail)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                await _emailService.SendEmailAsync(
                    user.Email,
                    "You have a notification from Ticksy!",
                    message
                );
            }
        }
    }
}