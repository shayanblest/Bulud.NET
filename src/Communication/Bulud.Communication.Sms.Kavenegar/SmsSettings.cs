namespace Bulud.Communication.Sms.KaveNegar;

public class SmsSettings
{
    public required bool IsActive { get; set; }
    public required string ApiKey { get; set; }
    public string? Sender { get; set; }
    public string? OtpTemplate { get; set; }
    public Dictionary<string, string> Templates { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
