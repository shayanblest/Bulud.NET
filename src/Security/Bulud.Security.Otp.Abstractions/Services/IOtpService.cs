namespace Bulud.Security.Otp.Abstractions.Services;

public interface IOtpService
{
    Task<string> GenerateAsync(string userIdentifier);
    Task<bool> VerifyAsync(string userIdentifier, string otp);
}
