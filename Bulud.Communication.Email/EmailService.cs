using Bulud.Base.Services;

namespace Bulud.Communication.Email;

public class EmailService : IEmailService
{
    public Task SendAsync(string email, string subject, string body)
    {
        throw new NotImplementedException();
    }
}