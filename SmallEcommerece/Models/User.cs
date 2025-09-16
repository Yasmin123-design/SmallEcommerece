using System.ComponentModel.DataAnnotations;

namespace SmallEcommerece.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }   

        [Required]
        public string PasswordHash { get; set; }   

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; }  

        public DateTime LastLoginTime { get; set; }
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
