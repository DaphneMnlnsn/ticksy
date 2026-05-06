namespace ticksy_api.Models
{
    public class Notification {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public enum NotifType {
            LeaveRequest,
            ForgotClockOut,
            LeaveApproved,
            LeaveRejected,
            AssignedToTeam
        }
        public NotifType Type { get; set; } = NotifType.AssignedToTeam;
        
        public enum NotificationStatus
        {
            Pending,
            Sent,
            Failed
        }

        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

        public required string Message { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}