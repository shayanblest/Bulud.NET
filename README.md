# Bulud.NET

A comprehensive collection of .NET libraries providing essential infrastructure components for building robust .NET applications.

## Packages

### Core Package

#### [Bulud.Base](Bulud.Base/README.md)
The foundational library providing:
- Custom exception handling with global middleware
- Extension methods for EF Core, Claims, Collections, and more
- Base infrastructure for Repository & DbContext patterns
- Data exporting to CSV and PDF
- Authorization helpers and JWT utilities
- JSON converters and validation helpers

### Communication Packages

#### [Bulud.Communication.Email](Bulud.Communication.Email/README.md)
Email service implementation for sending notifications via SMTP or other providers.

#### [Bulud.Communication.Sms.Kavenegar](Bulud.Communication.Sms.Kavenegar/README.md)
SMS service integration with Kavenegar API for sending text messages and OTP codes.

### File Storage Packages

#### [Bulud.FileStorage.Local](Bulud.FileStorage.Local/README.md)
Local file system storage implementation for development and simple deployments.

#### [Bulud.FileStorage.S3](Bulud.FileStorage.S3/README.md)
S3-compatible storage implementation using MinIO client for cloud file storage.

#### [Bulud.FileStorage.Azure](Bulud.FileStorage.Azure/README.md)
Azure Blob Storage implementation for Microsoft Azure cloud storage.

### Security Packages

#### [Bulud.Security.InMemoryOtp](Bulud.Security.InMemoryOtp/README.md)
In-memory OTP (One-Time Password) service for authentication and verification.

## Installation

Each package can be installed independently via NuGet:

```bash
# Core package
dotnet add package Bulud.Base

# Communication
dotnet add package Bulud.Communication.Email
dotnet add package Bulud.Communication.Sms.KaveNegar

# File Storage
dotnet add package Bulud.FileStorage.Local
dotnet add package Bulud.FileStorage.S3
dotnet add package Bulud.FileStorage.Azure

# Security
dotnet add package Bulud.Security.InMemoryOtp
```

## Quick Start

### Basic Setup with Bulud.Base

```csharp
// Program.cs or Startup.cs
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.Configure<ErrorHandlingSettings>(builder.Configuration.GetSection("ErrorHandlingSettings"));

// Add middleware
app.UseMiddleware<GlobalExceptionMiddleware>();
```

### Using Multiple Services

```csharp
// Register services
builder.Services.AddLocalFileService();
builder.Services.AddInMemoryOtpService(builder.Configuration);
builder.Services.AddEmail(builder.Configuration);

// Inject and use
public class MyService
{
    private readonly IFilesService _files;
    private readonly IOtpService _otp;
    private readonly IEmailService _email;

    public MyService(IFilesService files, IOtpService otp, IEmailService email)
    {
        _files = files;
        _otp = otp;
        _email = email;
    }
}
```

## Architecture

Bulud.NET follows a modular architecture where:

- **Bulud.Base** provides interfaces and base implementations
- **Extension packages** implement specific interfaces for different providers
- **Dependency Injection** is used for easy service registration and resolution
- **Configuration-based setup** allows flexible deployment options

## Contributing

Contributions are welcome! Please see our contributing guidelines and code of conduct.

## License

All packages are licensed under Apache-2.0.

## Support

For issues and questions, please use the GitHub issues page.