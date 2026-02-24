using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Api.DbObjects
{
    public class Media
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public string Extension { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsLiked { get; set; }
        //FK
        public Guid ParentFolderId { get; set; }
        public ProjectFolder ParentFolder { get; set; } = null!;

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
    }
}
