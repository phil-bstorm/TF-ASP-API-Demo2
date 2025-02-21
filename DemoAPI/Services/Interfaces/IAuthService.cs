using DemoAPI.Domain.Models;

namespace DemoAPI.Services.Interfaces
{
    public interface IAuthService
    {
        public string GenerateToken(Utilisateur utilisateur);
    }
}
