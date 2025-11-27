# Bulud.Communication.Email

A lightweight .NET library providing email notification service implementation for Bulud.Base interfaces.

## Features

- Send email messages via SMTP or other providers
- Fully compatible with Bulud.Base interfaces
- Suitable for production and testing environments
- Easy integration with .NET DI

## Installation

```bash
dotnet add package Bulud.Communication.Email
```

## Usage

### Dependency Injection Setup

```csharp
builder.Services.AddEmail(builder.Configuration);
```

### Sending Emails

```csharp
public class MyService
{
    private readonly IEmailService _emailService;

    public MyService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendWelcomeEmail(string userEmail)
    {
        await _emailService.SendAsync(userEmail, "Welcome!", "Welcome to our platform!");
    }
}
```

## Implementation Note

This package provides the interface implementation for `IEmailService` from Bulud.Base. The actual email sending logic needs to be implemented based on your email provider (SMTP, SendGrid, etc.).

## Dependencies

- Bulud.Base

## License

Apache-2.0