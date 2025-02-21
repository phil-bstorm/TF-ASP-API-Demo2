using DemoAPI.Domain.Models;
using DemoAPI.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DemoAPI.Services
{
    public class AuthService : IAuthService
    {
        // Injection de configuration car on a besoin des informations qui se trouvent dans le appsettings.json > Jwt
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Utilisateur utilisateur)
        {
            // Création d'objet de sécurtié avec les informations de l'utilisateur
            // /!\ NE PAS METTRE D'INFORMATIONS SENSIBLES /!\
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, utilisateur.Id.ToString()),
                new Claim(ClaimTypes.Role, utilisateur.Grade.ToString())

                /* NE SURTOUT PAS FAIRE: les informations sont sensibles
                 * Elle pourrait servir à hacker de pouvoir se générer un token et se faire passer pour la personne
                    new Claim("username", utilisateur.Username),
                    new Claim("password", utilisateur.Password)
                */
            };

            // Crédentials pour signé le token (clé + algo de cryptage)
            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Génération du token
            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddDays(42),
                signingCredentials: credentials
            );

            // Export du token en string
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
    }
}
