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
            AssignedToTeam
        }
        public NotifType Type { get; set; } = NotifType.AssignedToTeam;

        public required string Message { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}