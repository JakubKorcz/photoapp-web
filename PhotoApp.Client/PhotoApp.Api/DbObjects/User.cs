using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Api.DbObjects
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Username { get; set; } = string.Empty;
        public int? LoginCode { get; set; }
        public DateTime? CodeExpiration { get; set; }
        List<Project> Projects { get; set; } = new();
    }
}
