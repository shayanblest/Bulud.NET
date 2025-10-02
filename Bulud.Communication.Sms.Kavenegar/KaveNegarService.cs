using Bulud.Base.Services;
using Microsoft.Extensions.Options;

namespace Bulud.Communication.Sms.KaveNegar;

public class KaveNegarService(IOptions<SmsSettings> settings) : ISmsService
{
    private readonly HttpClient _httpClient = new();
    private readonly SmsSettings _settings = settings.Value;

    public Task SendAsync(string number, string message)
    {
        throw new NotImplementedException();
    }

    public async Task SendAsync(string number, string[] tokens)
    {
        var url = $"https://api.kavenegar.com/v1/{_settings.ApiKey}/verify/lookup.json" +
                  $"?receptor={number}&token={tokens[0]}&template={_settings.OtpTemplate}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Couldn't send sms");
        }
        
    }
}