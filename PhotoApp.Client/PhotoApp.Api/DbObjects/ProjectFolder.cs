using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Api.DbObjects
{
    public class ProjectFolder
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ProjectFolder> Folders { get; set; }
        public List<Media> Medias { get; set; }

        //FK
        public ProjectFolder Parent { get; set; }
        public Project Project { get; set; }

    }
}
