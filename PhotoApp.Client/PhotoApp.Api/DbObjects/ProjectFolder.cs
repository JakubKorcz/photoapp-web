using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Api.DbObjects
{
    public class ProjectFolder
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        //czy folder jest folderem głównym projektu, czy podfolderem. 
        public bool IsHeadFolder { get; set; } = false;

        public List<ProjectFolder> Folders { get; set; } = new();
        public List<Media> Medias { get; set; } = new();

        //FK
        public Guid? ParentFolderId { get; set; }
        public ProjectFolder? ParentFolder { get; set; }
    }
}
