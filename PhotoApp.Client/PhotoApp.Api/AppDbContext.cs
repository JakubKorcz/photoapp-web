using Microsoft.EntityFrameworkCore;
using PhotoApp.Api.DbObjects;

namespace PhotoApp.Api
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectFolder> Folders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            //Project
            modelBuilder.Entity<Project>()
                .HasKey(u => u.Id);

            //ProjectFolder
            modelBuilder.Entity<ProjectFolder>()
                .HasKey(u => u.Id);
        }
    }
}
