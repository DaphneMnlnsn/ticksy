namespace ticksy_api.Models
{
    public class User {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? PasswordHash { get; set; }

        public enum UserRole {
            User,
            Admin,
        }
        public UserRole Role { get; set; } = UserRole.User;

        public required string Phone { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public required string TimeZone { get; set; } = "Asia/Manila";
        public bool AutoClockIn { get; set; }
        public string? OauthProvider { get; set; }
        public string? OauthProviderId { get; set; }
        public bool EmailVerified { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public List<WorkSchedule> CreatedSchedules { get; set; } = [];
        public List<UserWorkSchedule> UserWorkSchedules { get; set; } = [];
        public List<TeamMember> TeamMemberships { get; set; } = [];
        public List<Team> CreatedTeams { get; set; } = [];
        public List<TeamInvite> CreatedInvites { get; set; } = [];
        public List<Report> GeneratedReports { get; set; } = [];
        public List<LeaveRequest> LeaveRequests { get; set; } = [];
        public List<LeaveRequest> ApprovedRequests { get; set; } = [];
        public List<Notification> Notifications { get; set; } = [];
        public List<Holiday> CreatedHolidays { get; set; } = [];
        public List<Attendance> Attendances { get; set; } = [];
    }
}