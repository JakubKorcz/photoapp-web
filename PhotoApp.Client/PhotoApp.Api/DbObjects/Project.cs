using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Api.DbObjects
{
    public class Project
    {
        public Guid Id { get; set; }
        public Guid Creator {  get; set; }
        public string ProjectName { get; set; }
        public DateOnly CreatedAt { get; set; }
        public List<ProjectFolder>? Folders { get; set; } = null;
        public List<Media>? Medias { get; set; } = null;
    }
}
