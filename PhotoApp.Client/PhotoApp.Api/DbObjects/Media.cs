using PhotoApp.Common.EnumShared;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

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
        public PhotoType Type { get; set; }
        public string ObjectKey { get; set; } = string.Empty;
        public bool IsLiked { get; set; }
        public long SizeBytes { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Media>? ConnectedMedias { get; set; }
        //FK
        public Guid? ParentFolderId { get; set; }
        public ProjectFolder ParentFolder { get; set; } = null!;

        public Guid? ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        //FK for media connected to other media eg. preview or thumbnail
        public Guid? ParentMediaId { get; set; }
        public Media ParentMedia { get; set; } = null!;
    }
}
