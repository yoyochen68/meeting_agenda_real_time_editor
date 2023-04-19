using Microsoft.EntityFrameworkCore;

namespace MeetingAgenda.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) :
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Meeting>()
                .Property(e => e.Created)
                .HasDefaultValueSql("now()");

            modelBuilder
                .Entity<Message>()
                .Property(e => e.Created)
                .HasDefaultValueSql("now()");
        }

        public DbSet<Meeting> Meetings => Set<Meeting>();

        public DbSet<Message> Messages => Set<Message>();
    }
}
