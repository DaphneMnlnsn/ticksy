namespace ticksy_api.Models
{
    public class TimeOffPolicy {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int MaxDays { get; set; }
        public required string Rules { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public List<LeaveRequest> LeaveRequests { get; set; } = [];
    }
}