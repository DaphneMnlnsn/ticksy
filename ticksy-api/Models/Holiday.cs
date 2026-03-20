using System.ComponentModel.DataAnnotations.Schema;

namespace ticksy_api.Models
{
    public class Holiday {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateOnly Date { get; set; }

        public enum HolidayType {
            Public,
            CompanySpecific,
            Imported
        }
        public HolidayType Type { get; set; } = HolidayType.Public;

        public enum HolidaySource {
            Manual,
            Google,
            Outlook
        }
        public HolidaySource Source { get; set; } = HolidaySource.Manual;

        public string? ExternalId { get; set; }

        public int CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public User CreatedByUser { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}