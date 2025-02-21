using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DemoAPI.DTOs
{
    public class CreateCarDTO
    {
        [MaxLength(50)]
        [Required]
        public required string Brand { get; set; }

        [MaxLength(50)]
        [Required]
        public string Model { get; set; }

        [MaxLength(50)]
        [Required]
        public required string Color { get; set; }

        [Required]
        public required int HorsePower { get; set; }

        public bool IsNew { get; set; }

        public int? OwnerId { get; set; }
    }
}
