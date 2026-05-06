using Microsoft.EntityFrameworkCore;
using ticksy_api.Models;
using System.Text.RegularExpressions;

namespace ticksy_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<TeamInvite> TeamInvites { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<WorkScheduleDay> WorkScheduleDays { get; set; }
        public DbSet<WorkScheduleBreak> WorkScheduleBreaks { get; set; }
        public DbSet<UserWorkSchedule> UserWorkSchedules { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<HolidayCalendar> HolidayCalendars { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<TimeOffPolicy> TimeOffPolicies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //LeaveRequest relationships
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.User)
                .WithMany(u => u.LeaveRequests)
                .HasForeignKey(lr => lr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.ReviewedByUser)
                .WithMany(u => u.ReviewedRequests)
                .HasForeignKey(lr => lr.ReviewedBy)
                .OnDelete(DeleteBehavior.Restrict);

            //Indexes and unique
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<TeamMember>().HasIndex(tm => new { tm.TeamId, tm.UserId }).IsUnique();
            modelBuilder.Entity<TeamInvite>().HasIndex(i => i.Token).IsUnique();
            modelBuilder.Entity<TeamInvite>().HasIndex(i => i.TeamId);
            modelBuilder.Entity<TeamInvite>().HasIndex(i => i.ExpiresAt);
            modelBuilder.Entity<Attendance>().HasIndex(a => new { a.UserId, a.Date });

            //Enum conversions
            modelBuilder.Entity<Attendance>().Property(a => a.ClockInSource).HasConversion<string>();
            modelBuilder.Entity<Holiday>().Property(h => h.Type).HasConversion<string>();
            modelBuilder.Entity<Holiday>().Property(h => h.Source).HasConversion<string>();
            modelBuilder.Entity<Notification>().Property(n => n.Type).HasConversion<string>();
            modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();
            modelBuilder.Entity<WorkSchedule>().Property(ws => ws.WorkArrangement).HasConversion<string>();
            modelBuilder.Entity<TeamMember>().Property(tm => tm.TeamRole).HasConversion<string>();
            modelBuilder.Entity<LeaveRequest>().Property(lr => lr.Status).HasConversion<string>();

            //Snake_case conversion
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(ToSnakeCase(entity.GetTableName()));

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }
            }
        }

        //UTC conversion for all DateTimes
        public override int SaveChanges()
        {
            SetUtcDateTimes();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetUtcDateTimes();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetUtcDateTimes()
        {
            foreach (var entry in ChangeTracker.Entries()
                        .Where(e => e.Entity != null &&
                                    (e.State == EntityState.Added || e.State == EntityState.Modified)))
            {
                foreach (var property in entry.Properties)
                {
                    if (property.CurrentValue == null)
                        continue;

                    if (property.Metadata.ClrType == typeof(DateTime))
                    {
                        var dt = (DateTime)property.CurrentValue;
                        if (dt.Kind == DateTimeKind.Unspecified || dt.Kind == DateTimeKind.Local)
                            property.CurrentValue = dt.ToUniversalTime();
                    }
                    else if (property.Metadata.ClrType == typeof(DateTime?))
                    {
                        var nullableDt = (DateTime?)property.CurrentValue;
                        if (nullableDt.HasValue && (nullableDt.Value.Kind == DateTimeKind.Unspecified || nullableDt.Value.Kind == DateTimeKind.Local))
                            property.CurrentValue = nullableDt.Value.ToUniversalTime();
                    }
                }
            }
        }

        private static string ToSnakeCase(string? input)
        {
            if (string.IsNullOrEmpty(input)) return input ?? "";

            var startUnderscores = Regex.Match(input, @"^_+").Value;
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

    }
}