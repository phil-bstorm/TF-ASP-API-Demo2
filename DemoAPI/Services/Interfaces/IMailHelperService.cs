using DemoAPI.Domain.Models;

namespace DemoAPI.Services.Interfaces
{
    public interface IMailHelperService
    {
        void SendWelcomeMail(Utilisateur utilisateur);
        void SendWarningLoginMail(Utilisateur utilisateur);
    }
}
