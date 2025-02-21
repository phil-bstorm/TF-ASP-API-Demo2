using System.ComponentModel.DataAnnotations;

namespace DemoAPI.DTOs
{
    public class LoginUtilisateurDTO
    {
        [Required]
        [MaxLength(30)]
        public required string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Password { get; set; }
    }
}
