namespace Bulud.Communication.Abstractions.Services;

public interface ISmsService
{
    Task SendAsync(string number, string message);
    Task SendAsync(string number, string[] tokens);
    Task SendAsync(string number, string[] tokens, string template);
    Task SendAsync(string number, IReadOnlyDictionary<string, string> tokens, string template);
}
