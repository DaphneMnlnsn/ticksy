using System.ComponentModel.DataAnnotations.Schema;

namespace ticksy_api.Models
{
    public class LeaveRequest {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int LeaveTypeId { get; set; }

        [ForeignKey("LeaveTypeId")]
        public TimeOffPolicy TimeOffPolicy { get; set; } = null!;

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public required string Reason { get; set; }
        public enum RequestStatus {
            Pending,
            Approved,
            Rejected,
            Cancelled
        }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public int? ReviewedBy { get; set; }
        
        [ForeignKey("ReviewedBy")]
        public User? ReviewedByUser { get; set; }

        public DateTime? ReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}