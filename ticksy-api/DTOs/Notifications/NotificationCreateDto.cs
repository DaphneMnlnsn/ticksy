using ticksy_api.Models;

public class NotificationCreateDto
{
    public int UserId { get; set; }
    public Notification.NotifType Type { get; set; }
    public string Message { get; set; } = null!;
}