using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Dto
{
    public sealed class RegisterDto
    {
        [Length(1, 100), Required]
        public string Name { get; set; } = string.Empty;
        [Length(1, 100), Required]
        public string Surname { get; set; } = string.Empty;
        [Length(1, 100), EmailAddress, Required]
        public string Email { get; set; } = string.Empty;
        [MinLength(8), Required]
        public string Password { get; set; } = string.Empty;
        [MinLength(8), Required]
        public string ConfirmedPassword { get; set; } = string.Empty;
    }
}
