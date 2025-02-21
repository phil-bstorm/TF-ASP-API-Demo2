using System.ComponentModel.DataAnnotations;

namespace DemoAPI.DTOs
{
    public class UpdateUtilisateurDTO
    {
        [MaxLength(50)]
        public string? Username { get; set; }


        [MaxLength(50)]
        public string? Password { get; set; }
    }
}
