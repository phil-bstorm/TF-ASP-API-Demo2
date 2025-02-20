using DemoAPI.Domain.CustomEnums;
using DemoAPI.Domain.Models;
using DemoAPI.DTOs;

namespace DemoAPI.Mappers
{
    public static class UtilisateursMappers
    {
        public static ListUtilisateurDTO ToListUtilisateurDTO(this Utilisateur utilisateur)
        {
            return new ListUtilisateurDTO
            {
                Id = utilisateur.Id,
                Username = utilisateur.Username,
            };
        }

        public static Utilisateur ToUtilisateur(this CreateUtilisateurDTO dto) {
            return new Utilisateur
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
                Grade = (Grade)Enum.Parse(typeof(Grade), dto.Grade)
            };
        }

        public static DetailsUtilisateurDTO ToDetailsUtilisateurDTO(this Utilisateur utilisateur) {
            return new DetailsUtilisateurDTO
            {
                Id = utilisateur.Id,
                Email = utilisateur.Email,
                Username = utilisateur.Username,
                Grade = utilisateur.Grade.ToString(),
            };
        }
    }
}
