using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Api.DbObjects
{
    public class ProjectFolder
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        //FK
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }

    }
}
