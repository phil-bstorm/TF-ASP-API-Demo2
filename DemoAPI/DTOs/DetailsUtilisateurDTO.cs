using DemoAPI.Domain.CustomEnums;

namespace DemoAPI.DTOs
{
    public class DetailsUtilisateurDTO
    {
        public required int Id { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }

        public required string Grade { get; set; }

        public List<ListCarDTO> Cars { get; set; } = [];
    }
}
