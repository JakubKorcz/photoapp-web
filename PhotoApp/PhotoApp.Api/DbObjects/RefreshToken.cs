using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Api.DbObjects
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        [Required]
        public string Username { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
