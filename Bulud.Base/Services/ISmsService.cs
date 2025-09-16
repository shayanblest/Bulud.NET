namespace Bulud.Base.Services;

public interface ISmsService
{
    Task SendAsync(string number, string message);
    Task SendAsync(string number,string[] tokens);
}