namespace ticksy_api.Models
{
    public class Attendance {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateOnly Date { get; set; }
        public TimeOnly TimeIn { get; set; }
        public TimeSpan BreakDuration { get; set; }
        public TimeOnly? TimeOut { get; set; }
        public int LateMinutes  { get; set; }
        public bool ManuallyEntered { get; set; }
        public enum ClockInSourceEnum {
            Manual,
            AutoLogin,
            Api
        }
        public ClockInSourceEnum ClockInSource { get; set; } = ClockInSourceEnum.Manual;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}