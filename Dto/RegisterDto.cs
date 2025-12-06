using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public class RegisterDto
    {
        [Required, Length(1, 100)]
        public string Name { get; set; } = string.Empty;
        [Required, Length(1, 100)]
        public string Surname { get; set; } = string.Empty;
        [Required, Length(1, 100), EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string ConfirmedPassword { get; set; } = string.Empty;
    }
}
