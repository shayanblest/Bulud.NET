# Bulud.Communication.Sms.Kavenegar

A lightweight .NET library providing integration with the Kavenegar SMS service for Bulud.Communication.Abstractions interfaces.

## Features

- Send SMS messages via Kavenegar API
- Fully compatible with Bulud.Communication.Abstractions interfaces
- Reliable for production and real-world scenarios
- Easy integration with .NET DI

## Installation

```bash
dotnet add package Bulud.Communication.Sms.KaveNegar
```

## Usage

### Configuration

Add SMS settings to your `appsettings.json`:

```json
{
  "SmsSettings": {
    "IsActive": true,
    "ApiKey": "your-kavenegar-api-key",
    "Sender": "your-kavenegar-sender-line",
    "Templates": {
      "Otp": "your-otp-template-name",
      "Order": "your-order-template-name"
    }
  }
}
```

### Dependency Injection Setup

```csharp
builder.Services.AddKaveNegar(builder.Configuration);
```

### Sending SMS

```csharp
public class MyService
{
    private readonly ISmsService _smsService;

    public MyService(ISmsService smsService)
    {
        _smsService = smsService;
    }

    public async Task SendOtp(string phoneNumber, string[] tokens)
    {
        await _smsService.SendAsync(phoneNumber, tokens);
    }

    public async Task SendOrder(string phoneNumber, string[] tokens)
    {
        await _smsService.SendAsync(phoneNumber, tokens, "Order");
    }

    public async Task SendMessage(string phoneNumber, string message)
    {
        await _smsService.SendAsync(phoneNumber, message);
    }
}
```

## API Methods

- `SendAsync(string number, string message)`: Send plain text SMS (requires `SmsSettings:Sender`)
- `SendAsync(string number, string[] tokens)`: Send using the `Otp` template
- `SendAsync(string number, string[] tokens, string template)`: Send using the selected configured template
- `SendAsync(string number, IReadOnlyDictionary<string, string> tokens, string template)`: Send named `token`, `token2`, `token3`, `token10`, or `token20` fields

Use `SendAsync(number, message)` for free-form text such as Persian order statuses. It uses Kavenegar's regular SMS endpoint. Lookup templates use the token fields supported by the configured Kavenegar pattern. The old `OtpTemplate` setting is still supported as a fallback when `Templates:Otp` is absent.

## Dependencies

- Bulud.Communication.Abstractions
- Microsoft.Extensions.Options

## License

Apache-2.0
