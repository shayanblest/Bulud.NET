# Bulud.Security.InMemoryOtp

A lightweight .NET library providing an in-memory implementation of OTP (One-Time Password) service for Bulud.Base interfaces.

## Features

- Generate and validate OTP codes in memory
- Fully compatible with Bulud.Base interfaces
- Simple and fast for testing or lightweight applications
- Easy integration with .NET DI

## Installation

```bash
dotnet add package Bulud.Security.InMemoryOtp
```

## Usage

### Configuration

Add OTP settings to your `appsettings.json`:

```json
{
  "OtpSettings": {
    "CodeLength": 6,
    "ExpireMinutes": 2
  }
}
```

### Dependency Injection Setup

```csharp
builder.Services.AddInMemoryOtpService(builder.Configuration);
```

### OTP Operations

```csharp
public class AuthService
{
    private readonly IOtpService _otpService;

    public AuthService(IOtpService otpService)
    {
        _otpService = otpService;
    }

    public async Task<string> GenerateOtp(string userId)
    {
        return await _otpService.GenerateAsync(userId);
    }

    public async Task<bool> VerifyOtp(string userId, string otpCode)
    {
        return await _otpService.VerifyAsync(userId, otpCode);
    }
}
```

## Features

- Generates random numeric OTP codes
- Stores OTP codes in memory with expiration
- Automatic cleanup after verification or expiration
- Configurable code length and expiration time

## Dependencies

- Bulud.Base
- Microsoft.Extensions.Caching.Abstractions
- Microsoft.Extensions.Options

## License

Apache-2.0