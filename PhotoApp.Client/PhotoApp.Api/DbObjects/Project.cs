using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Api.DbObjects
{
    public class Project
    {
        public Guid Id { get; set; }
        public Guid Creator {  get; set; }
        public Guid Destination { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public bool IsLiked { get; set; }
        public List<ProjectFolder> Folders {  get; set; }
        public List<Photo> Photos { get; set; }
    }
}
