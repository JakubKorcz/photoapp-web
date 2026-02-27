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
        public DbSet<Media> Medias { get; set; }
        public DbSet<ProjectWebDesign> WebDesignes { get; set; }
        public DbSet<Project_ProjectWebDesign> Project_ProjectWebDesigns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Username musi być unikalne
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            //ProjectID i Device muszą być unikalne w tabeli Project_ProjectWebDesign czyli dla kazdego projektu istnieje tyle webdesignow ile jest urzadzen
            modelBuilder.Entity<Project_ProjectWebDesign>()
                .HasIndex(ppwd => new { ppwd.ProjectId, ppwd.Device })
                .IsUnique();

            //Klucze obce w tabeli pośredniej Project_ProjectWebDesign
            modelBuilder.Entity<Project_ProjectWebDesign>()
                .HasOne(ppwd => ppwd.Project)
                .WithMany() 
                .HasForeignKey(ppwd => ppwd.ProjectId);

            modelBuilder.Entity<Project_ProjectWebDesign>()
                .HasOne(ppwd => ppwd.ProjectWebDesign)
                .WithMany()
                .HasForeignKey(ppwd => ppwd.ProjectWebDesignId);


            base.OnModelCreating(modelBuilder);
        }
    }
}
