namespace Bulud.Communication.Abstractions.Services;

public interface IEmailService
{
    Task SendAsync(string email, string subject, string body);
}
