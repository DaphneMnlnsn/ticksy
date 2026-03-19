namespace ticksy_api.Models
{
    public class TeamMember {
        public int Id { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime JoinedAt { get; set; }
        public enum TeamRoleEnum {
            Member,
            Leader
        }
        public TeamRoleEnum TeamRole { get; set; } = TeamRoleEnum.Member;
    }
}