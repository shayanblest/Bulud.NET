# Bulud.Authentication.Jwt

JWT authentication configuration and registration for ASP.NET Core applications.

## Installation

```bash
dotnet add package Bulud.Authentication.Jwt
```

## Registration

Register JWT authentication during service configuration:

```csharp
builder.Services.AddJwtAuthentication(builder.Configuration);
```

The registration uses the standard JWT bearer authentication scheme for the
default authenticate, challenge, and scheme settings.

## Configuration

Configure `JwtSettings` in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "ExpireMinutes": 60
  }
}
```

## Web error handling

Error-handling configuration is independent of JWT registration. When the
application uses `Bulud.AspNetCore`, register it explicitly and add its
middleware to the request pipeline:

```csharp
builder.Services.AddErrorHandling(builder.Configuration);
app.UseMiddleware<GlobalExceptionMiddleware>();
```
