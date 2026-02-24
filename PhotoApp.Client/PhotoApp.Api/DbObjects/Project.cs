using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Api.DbObjects
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ProjectName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now.Date;

        //FK
        [Required]
        public Guid MainFolderId { get; set; }
        public ProjectFolder MainFolder { get; set; } = null!;

        [Required]
        public Guid UserId { get; set; }
        public User Owner { get; set; } = null!;

        public List<Project_ProjectWebDesign> WebDesignAssignments { get; set; } = new();

    }
}
