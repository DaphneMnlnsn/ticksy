namespace ticksy_api.Models
{
    public class UserWorkSchedule {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int ScheduleId { get; set; }
        public WorkSchedule WorkSchedule { get; set; } = null!;

        public TimeOnly? CustomStartTime { get; set; }
        public TimeOnly? CustomEndTime { get; set; }
        public TimeSpan? CustomBreakDuration { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
    }
}