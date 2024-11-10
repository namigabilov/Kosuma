using Kosuma.Models;
using Microsoft.EntityFrameworkCore;

namespace Kosuma.Db
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=namig;Password=Namigna065;Database=LiveStream;");
        }

        public DbSet<LiveStream> LiveStreams { get; set; }
        public DbSet<LiveChat> LiveChats { get; set; }
        public DbSet<User> Users { get; set; }
    }
}