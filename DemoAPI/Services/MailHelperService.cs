using DemoAPI.Domain.Models;
using DemoAPI.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace DemoAPI.Services
{
    public class MailHelperService : IMailHelperService
    {
        private readonly string _noReplyName;
        private readonly string _noReplyEmail;
        private readonly string _smtpHost;
        private readonly int _smptPort;

        public MailHelperService(IConfiguration configuration)
        {
            _noReplyName = configuration.GetValue<string>("Smtp:NoReply:Name")!;
            _noReplyEmail = configuration.GetValue<string>("Smtp:NoReply:Email")!;
            _smtpHost = configuration.GetValue<string>("Smtp:Host")!;
            _smptPort = configuration.GetValue<int>("Smpt:Port");
        }

        private SmtpClient GetSmtpClient()
        {
            // connection vers le SMTP
            SmtpClient client = new SmtpClient();
            client.Connect(_smtpHost, _smptPort);
            // client.Authenticate(...); // si besoin d'une authentification avec le server SMTP
            return client;
        }

        public void SendWelcomeMail(Utilisateur utilisateur)
        {
            // préparation de l'email
            MimeMessage email = new MimeMessage();
            email.From.Add(new MailboxAddress(_noReplyName, _noReplyEmail));
            email.To.Add(new MailboxAddress(utilisateur.Username, utilisateur.Email));
            email.Subject = "Bienvenue sur notre super site !";
            email.Body = new TextPart(TextFormat.Plain)
            {
                Text = $"Bienvenue dans notre site, {utilisateur.Username}! \n\n" +
                        "╰(*°▽°*)╯ \n\n" +
                        "Coordialement l'équipe Demo."
            };

            using SmtpClient client = GetSmtpClient();

            client.Send(email);
            client.Disconnect(true);
        }

        public void SendWarningLoginMail(Utilisateur utilisateur)
        {
            // préparation de l'email
            MimeMessage email = new MimeMessage();
            email.From.Add(new MailboxAddress(_noReplyName, _noReplyEmail));
            email.To.Add(new MailboxAddress(utilisateur.Username, utilisateur.Email));
            email.Subject = "Attention une connection à votre compte a été detectée";
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = $"<!DOCTYPE html><html lang=\"fr\">" +
                $"<body>" +
                $"<table>" +
                $"<tr>" +
                $"<td>" +
                $"Connection de detecté" +
                $"</td>" +
                 $"<td>" +
                $"{utilisateur.Username}" +
                $"</td>" +
                $"</tr>" +
                $"</table>" +
                $"</body>" +
                $"</html>"
            };

            // connection vers le SMTP
            using SmtpClient client = GetSmtpClient();

            client.Send(email);
            client.Disconnect(true);
        }
    }
}
