# Bulud.Communication.Sms.Kavenegar

A lightweight .NET library providing integration with the Kavenegar SMS service for Bulud.Base interfaces.

## Features

- Send SMS messages via Kavenegar API
- Fully compatible with Bulud.Base interfaces
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
    "OtpTemplate": "your-otp-template-name"
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

    public async Task SendMessage(string phoneNumber, string message)
    {
        await _smsService.SendAsync(phoneNumber, message);
    }
}
```

## API Methods

- `SendAsync(string number, string message)`: Send a plain text SMS
- `SendAsync(string number, string[] tokens)`: Send OTP using template with tokens

## Dependencies

- Bulud.Base
- Microsoft.Extensions.Options

## License

Apache-2.0