using System.ComponentModel.DataAnnotations.Schema;

namespace ticksy_api.Models
{
    public class LeaveRequest {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int LeaveTypeId { get; set; }
        public TimeOffPolicy TimeOffPolicy { get; set; } = null!;

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public required string Reason { get; set; }
        public enum RequestStatus {
            Pending,
            Approved,
            Rejected
        }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public int? ApprovedBy { get; set; }
        
        [ForeignKey("ApprovedBy")]
        public User? ApprovedByUser { get; set; }

        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}