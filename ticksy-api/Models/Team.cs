using System.ComponentModel.DataAnnotations.Schema;

namespace ticksy_api.Models
{
    public class Team {
        public int Id { get; set; }
        public required string TeamName { get; set; }
        public int CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public User CreatedByUser { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public List<TeamMember> TeamMembers { get; set; } = [];
        public List<Team> TeamInvites { get; set; } = [];
    }
}