namespace ticksy_api.Models
{
    public class Report {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }

        public int GeneratedBy { get; set; }
        public User GeneratedByUser { get; set; } = null!;

        public required string Format { get; set; }
        public required string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}