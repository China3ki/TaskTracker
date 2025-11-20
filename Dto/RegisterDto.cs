using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Dto
{
    public sealed class RegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        [Length(1, 100), EmailAddress]
        public string Email { get; set; } = string.Empty;
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
        [MinLength(8)]
        public string ConfirmedPassword { get; set; } = string.Empty;
    }
}
