using VolunteerMgt.Server.Abstraction.Service.Common;

namespace VolunteerMgt.Server.Abstraction.Mail
{
    public interface IMailService : IScopedService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
