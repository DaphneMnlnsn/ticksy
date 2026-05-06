namespace ticksy_api.Models
{
    public class Attendance {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateOnly Date { get; set; }
        public TimeOnly TimeIn { get; set; }
        public TimeOnly? BreakStart { get; set; }
        public TimeSpan BreakDuration { get; set; } = TimeSpan.Zero;
        public TimeOnly? TimeOut { get; set; }
        public int LateMinutes  { get; set; }
        public int TotalWorkMinutes { get; set; }

        public enum ClockInSourceEnum {
            Manual,
            AutoLogin
        }
        public ClockInSourceEnum ClockInSource { get; set; } = ClockInSourceEnum.Manual;

        public enum AttendanceStatus
        {
            Present,
            Late,
            Absent,
            HalfDay
        }

        public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent;

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}