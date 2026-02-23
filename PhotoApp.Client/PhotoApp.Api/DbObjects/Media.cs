using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Api.DbObjects
{
    public class Media
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Extension { get; set; }
        public string Url { get; set; }
        public bool IsLiked { get; set; }
        //FK
        public ProjectFolder ProjectFolder { get; set; }
        public Project Project { get; set; }
    }
}
