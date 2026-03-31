using ticksy_api.Models;

public class NotificationListDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public Notification.NotifType Type { get; set; }
    public string Message { get; set; } = null!;
    public Notification.NotificationStatus Status { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}