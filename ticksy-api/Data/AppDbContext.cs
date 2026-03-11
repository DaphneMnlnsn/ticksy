using Microsoft.EntityFrameworkCore;

namespace ticksy_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Example table (you can delete later)
        //public DbSet<User> Users { get; set; }
    }
}