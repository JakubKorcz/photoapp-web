using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Client.Models
{
    public class UserModel : IValidatableObject
    {
        [EmailAddress(ErrorMessage = "Niepoprawny format email")]
        public string Email { get; set; }
        public string Username { get; set; }
        [MinLength(4, ErrorMessage = "Hasło musi mieć co najmniej 4 znaki")]
        [Required(ErrorMessage = "Hasło jest wymagane")]
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Username))
            {
                yield return new ValidationResult(
                    "Musisz podać Email lub Nazwę użytkownika.",
                    new[] { nameof(Email), nameof(Username) });
            }
        }
    }
}
