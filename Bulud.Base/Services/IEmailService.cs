namespace Bulud.Base.Services;

public interface IEmailService
{
    Task SendAsync(string email, string subject, string body);
}