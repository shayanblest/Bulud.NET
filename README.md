# Bulud.NET

A comprehensive collection of .NET libraries providing essential infrastructure components for building robust .NET applications.

## Packages

### Core Package

#### `Bulud.Core`
Dependency-free primitives, exceptions, converters, and common extensions.

### Communication Packages

#### `Bulud.Communication.Abstractions`
Contracts shared by communication providers.

#### [Bulud.Communication.Email](src/Communication/Bulud.Communication.Email/README.md)
Email service implementation for sending notifications via SMTP or other providers.

#### [Bulud.Communication.Sms.Kavenegar](src/Communication/Bulud.Communication.Sms.Kavenegar/README.md)
SMS service integration with Kavenegar API for sending text messages and OTP codes.

### File Storage Packages

#### `Bulud.FileStorage.Abstractions`
Storage contracts shared by local and S3 providers.

#### [Bulud.FileStorage.Local](src/FileStorage/Bulud.FileStorage.Local/README.md)
Local file system storage implementation for development and simple deployments.

#### [Bulud.FileStorage.S3](src/FileStorage/Bulud.FileStorage.S3/README.md)
S3-compatible storage implementation using MinIO client for cloud file storage.

#### [Bulud.FileStorage.Azure](src/FileStorage/Bulud.FileStorage.Azure/README.md)
Reserved, non-packable boundary for a future Azure provider.

### Security Packages

#### `Bulud.Security.Otp.Abstractions`
OTP contracts shared by OTP providers.

#### [Bulud.Security.Otp.InMemory](src/Security/Bulud.Security.Otp.InMemory/README.md)
In-memory OTP (One-Time Password) service for authentication and verification.

### Authentication Packages

#### [Bulud.Authentication.Jwt](src/Authentication/Bulud.Authentication.Jwt/README.md)
JWT authentication configuration and ASP.NET Core authentication registration.

### Persistence and Web Packages

#### `Bulud.EntityFrameworkCore`
Entity Framework Core persistence infrastructure, repositories, and query helpers.

#### `Bulud.AspNetCore`
ASP.NET Core middleware, authorization, and web-host integration.

### Exporting Packages

#### `Bulud.Exporting.Abstractions`
Exporter contracts shared by export implementations.

#### `Bulud.Exporting`
Export orchestration and provider selection.

#### `Bulud.Exporting.Csv`
CSV export provider.

#### `Bulud.Exporting.Pdf`
PDF export provider.

## Installation

Each package can be installed independently via NuGet; the following are common examples:

```bash
# Core
dotnet add package Bulud.Core

# Communication
dotnet add package Bulud.Communication.Email
dotnet add package Bulud.Communication.Sms.KaveNegar

# File Storage
dotnet add package Bulud.FileStorage.Local
dotnet add package Bulud.FileStorage.S3

# Security
dotnet add package Bulud.Security.Otp.InMemory

# Authentication
dotnet add package Bulud.Authentication.Jwt

# Persistence, web, and exporting
dotnet add package Bulud.EntityFrameworkCore
dotnet add package Bulud.AspNetCore
dotnet add package Bulud.Exporting
```

## Quick Start

### JWT and Web Setup

```csharp
// Program.cs or Startup.cs
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddErrorHandling(builder.Configuration);

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

- **Bulud.Core** contains dependency-free primitives
- **Abstraction packages** provide contracts shared by their providers
- **Provider and framework-integration packages** own their registration and SDK dependencies
- **Dependency Injection** and configuration provide explicit service registration

## Contributing

Contributions are welcome. The repository uses a pull-request-only workflow for `develop`:

1. Create a branch from the current `develop` head for every change.
2. Commit and push only to that change branch; do not commit or push directly to `develop`.
3. Open a pull request targeting `develop`, with the related issue and validation evidence.
4. Only the repository owner merges pull requests into `develop`.

See [RULE.md](RULE.md) for the complete delivery workflow and [AGENTS.md](AGENTS.md) for repository guidance.

## License

All packages are licensed under Apache-2.0.

## Support

For issues and questions, please use the GitHub issues page.
