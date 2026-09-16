namespace Bulud.Communication.Sms.KaveNegar;

public class SmsSettings
{
    public required bool IsActive { get; set; }
    public required string ApiKey { get; set; }
    public required string OtpTemplate { get; set; }
}