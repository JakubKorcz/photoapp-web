using Microsoft.EntityFrameworkCore;
using PhotoApp.Api.Objects;

namespace PhotoApp.Api
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=postgres;Username=postgres;Password=dupa123");
        }
    }
}
