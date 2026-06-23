using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Api.DbObjects
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? EmailLoginCode { get; set; }
        public DateTime? EmailLoginCodeExpiration { get; set; }
        public List<Project> Projects { get; set; } = new();
        public List<RefreshToken> RefreshTokens { get; set; } = new();
        //Flaga oznaczająca, czy konto po rejestracji zostało aktywowane przez użytkownika (np. poprzez kliknięcie linku w emailu)
        public bool IsActive { get; set; }
        public int FailedLoginCodeAttempts { get; set; }
        public DateTime? LoginCodeLockoutUntil { get; set; }

        public bool IsLoginCodeLockedOut => LoginCodeLockoutUntil.HasValue
            && DateTime.UtcNow < LoginCodeLockoutUntil.Value;

        public bool HasValidLoginCode(string code)
        {
            return !string.IsNullOrEmpty(code) &&
                   EmailLoginCode == code &&
                   DateTime.UtcNow <= EmailLoginCodeExpiration;
        }
    }
}
