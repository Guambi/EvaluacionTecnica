using System.ComponentModel.DataAnnotations;

namespace AuthService.Business.Dtos.Auth
{
    public class TokenRequestDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
