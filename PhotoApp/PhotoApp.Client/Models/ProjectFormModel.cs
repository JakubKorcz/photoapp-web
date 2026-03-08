using PhotoApp.Common.EnumShared;
using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Client.Models
{
    public class ProjectFormModel
    {
        [Required(ErrorMessage = "Nazwa projektu jest wymagana")]
        public string ProjectName { get; set; } = "";
        public DateOnly? PhotoShootDate { get; set; }
        public bool IsPernament { get; set; } = false;
        public DateTime? ExpiryDate { get; set; }
        public Language Language { get; set; } = Language.PL;
        [StringLength(4)]
        public string? Password { get; set; }
    }
}
