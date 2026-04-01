using System.ComponentModel.DataAnnotations.Schema;

namespace ticksy_api.Models
{
    public class WorkSchedule {
        public int Id { get; set; }
        public required string ScheduleName { get; set; }

        public enum WorkArrangementType {
            Fixed,
            Flexible,
            Weekly
        }
        public WorkArrangementType WorkArrangement { get; set; } =  WorkArrangementType.Fixed;
        public TimeSpan? WeeklyDuration { get; set; }

        public int CreatedBy { get; set; }
        
        [ForeignKey("CreatedBy")]
        public User CreatedByUser { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public List<WorkScheduleDay> ScheduleDays { get; set; } = [];
        public List<UserWorkSchedule> UserWorkSchedules { get; set; } = [];
    }
}