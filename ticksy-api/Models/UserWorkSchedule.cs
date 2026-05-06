namespace ticksy_api.Models
{
    public class UserWorkSchedule {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int WorkScheduleId { get; set; }
        public WorkSchedule WorkSchedule { get; set; } = null!;
        public DateTime AssignedAt { get; set; }
    }
}