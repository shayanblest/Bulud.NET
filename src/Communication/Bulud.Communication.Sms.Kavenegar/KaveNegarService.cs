using System.Text.Json;
using Bulud.Communication.Abstractions.Services;
using Microsoft.Extensions.Options;

namespace Bulud.Communication.Sms.KaveNegar;

public class KaveNegarService(IOptions<SmsSettings> settings) : ISmsService
{
    private static readonly HashSet<string> SupportedTokenNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "token", "token2", "token3", "token10", "token20"
    };

    private static readonly HttpClient HttpClient = new();
    private readonly SmsSettings _settings = settings.Value;

    public async Task SendAsync(string number, string message)
    {
        EnsureActive();
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("A phone number is required.", nameof(number));
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("A message is required.", nameof(message));

        var sender = _settings.Sender;
        if (string.IsNullOrWhiteSpace(sender))
            throw new InvalidOperationException("SmsSettings:Sender is required to send plain text SMS.");

        await SendRequestAsync("sms/send.json", new Dictionary<string, string>
        {
            ["receptor"] = number,
            ["sender"] = sender,
            ["message"] = message
        });
    }

    public Task SendAsync(string number, string[] tokens)
    {
        return SendAsync(number, tokens, "Otp");
    }

    public async Task SendAsync(string number, string[] tokens, string template)
    {
        ArgumentNullException.ThrowIfNull(tokens);
        if (tokens.Length == 0)
            throw new ArgumentException("At least one token is required.", nameof(tokens));
        if (tokens.Length > 3)
            throw new ArgumentException(
                "The array overload supports token, token2, and token3. Use the dictionary overload for token10 or token20.",
                nameof(tokens));

        var tokenValues = new Dictionary<string, string> { ["token"] = tokens[0] };
        if (tokens.Length > 1)
            tokenValues["token2"] = tokens[1];
        if (tokens.Length > 2)
            tokenValues["token3"] = tokens[2];

        await SendAsync(number, tokenValues, template);
    }

    public async Task SendAsync(
        string number,
        IReadOnlyDictionary<string, string> tokens,
        string template)
    {
        EnsureActive();
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("A phone number is required.", nameof(number));
        ArgumentNullException.ThrowIfNull(tokens);
        if (string.IsNullOrWhiteSpace(template))
            throw new ArgumentException("A configured template key is required.", nameof(template));
        if (tokens.Count == 0 ||
            !tokens.Keys.Any(key => string.Equals(key, "token", StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("The token dictionary must contain the required 'token' value.", nameof(tokens));
        }

        var templateName = ResolveTemplate(template);
        if (string.IsNullOrWhiteSpace(templateName))
            throw new ArgumentException(
                $"Template '{template}' is missing or empty in SmsSettings:Templates.",
                nameof(template));

        var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["receptor"] = number,
            ["template"] = templateName
        };
        foreach (var (key, value) in tokens)
        {
            if (!SupportedTokenNames.Contains(key))
                throw new ArgumentException(
                    $"'{key}' is not a supported Kavenegar VerifyLookup token field.",
                    nameof(tokens));
            if (value is null)
                throw new ArgumentException($"Token '{key}' cannot be null.", nameof(tokens));
            parameters[key.ToLowerInvariant()] = value;
        }

        await SendRequestAsync("verify/lookup.json", parameters);
    }

    private async Task SendRequestAsync(string endpoint, Dictionary<string, string> parameters)
    {
        EnsureActive();
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            throw new InvalidOperationException("SmsSettings:ApiKey is not configured.");

        var url = $"https://api.kavenegar.com/v1/{Uri.EscapeDataString(_settings.ApiKey)}/{endpoint}";
        using var content = new FormUrlEncodedContent(parameters);
        using var response = await HttpClient.PostAsync(url, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"Kavenegar returned HTTP {(int)response.StatusCode} ({response.StatusCode}): {responseBody}");

        try
        {
            using var json = JsonDocument.Parse(responseBody);
            var responseRoot = json.RootElement.ValueKind == JsonValueKind.Array &&
                               json.RootElement.GetArrayLength() > 0
                ? json.RootElement[0]
                : json.RootElement;

            if (responseRoot.ValueKind != JsonValueKind.Object ||
                !responseRoot.TryGetProperty("return", out var result) ||
                result.ValueKind != JsonValueKind.Object ||
                !result.TryGetProperty("status", out var statusElement) ||
                !statusElement.TryGetInt32(out var status))
            {
                throw new HttpRequestException($"Kavenegar returned an unexpected response: {responseBody}");
            }

            if (status != 200)
            {
                var message = result.TryGetProperty("message", out var messageElement)
                    ? messageElement.GetString()
                    : null;
                throw new HttpRequestException($"Kavenegar error {status}: {message ?? responseBody}");
            }
        }
        catch (JsonException)
        {
            throw new HttpRequestException($"Kavenegar returned an unreadable response: {responseBody}");
        }
    }

    private string? ResolveTemplate(string template)
    {
        if (_settings.Templates.TryGetValue(template, out var configuredTemplate))
            return configuredTemplate;

        if (string.Equals(template, "Otp", StringComparison.OrdinalIgnoreCase))
            return _settings.OtpTemplate;

        return null;
    }

    private void EnsureActive()
    {
        if (!_settings.IsActive)
            throw new InvalidOperationException("Kavenegar SMS is disabled by SmsSettings:IsActive.");
    }
}
