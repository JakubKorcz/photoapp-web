using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Api.DbObjects
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; }
        public Guid Owner {  get; set; }
        public string ProjectName { get; set; }
        public DateOnly CreatedAt { get; set; }
        //FK
        public ProjectFolder MainFolder { get; set; }
    }
}
