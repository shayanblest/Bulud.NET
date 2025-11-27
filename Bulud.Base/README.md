# Bulud.Base

A comprehensive utility library for .NET applications providing essential infrastructure components, exception handling, extensions, and more.

## Features

- **Custom Exception Handling**: Pre-built exceptions for common scenarios (Validation, Forbidden, NotFound, DuplicateRequest)
- **Global Exception Middleware**: Integrated with Serilog and Loki for logging and monitoring
- **Extension Methods**: Utilities for EF Core, Claims, Collections, Dates, and more
- **Base Infrastructure**: Repository pattern and DbContext base classes
- **Data Exporting**: CSV and PDF export capabilities using CsvHelper and QuestPDF
- **Authorization Helpers**: Permission-based authorization with JWT utilities
- **JSON Converters**: UTC DateTime converter for consistent serialization

## Installation

```bash
dotnet add package Bulud.Base
```

## Usage

### Dependency Injection Setup

```csharp
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.Configure<ErrorHandlingSettings>(builder.Configuration.GetSection("ErrorHandlingSettings"));
```

### JWT Authentication

Configure JWT settings in `appsettings.json`:

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

### Exception Handling

The library provides several custom exceptions:

```csharp
throw new AppValidationException(new Dictionary<string, string[]> { { "field", ["error"] } });
throw new ForbiddenException();
throw new NotFoundException();
throw new DuplicateRequestException();
```

### Global Exception Middleware

Add to your pipeline:

```csharp
app.UseMiddleware<GlobalExceptionMiddleware>();
```

### Extension Methods

```csharp
// Claims extensions
var userId = User.GetUserId();
var userName = User.GetUserName();

// Collection extensions
bool isEmpty = list.IsNullOrEmpty();

// Date extensions
var days = DateOnly.Subtract(endDate, startDate);

// Expression extensions
var propertyExpression = ExpressionExtensions.BuildNestedProperty(parameter, "Property.NestedProperty");

// File validation
var validation = file.ValidateFile(maxSizeBytes: 5 * 1024 * 1024); // 5MB
if (!validation.IsValid) {
    // Handle error
}

// MIME type detection
var mimeType = "file.pdf".GetMimeType();

// Validation helper
var results = ValidationHelper.Validate(dto);
```

### Repository Pattern

```csharp
public interface IMyRepository : IRepository<MyEntity> { }

public class MyRepository : IMyRepository {
    // Implementation
}
```

### Query Extensions

```csharp
var query = context.Entities
    .ApplyFilters("name:like(john)")
    .ApplySearch("email:eq(user@example.com)")
    .ApplySorting("createdAt(desc)")
    .ApplyIncludes("RelatedEntity")
    .ApplyPaging(1, 10);
```

### Data Exporting

```csharp
var exporter = new ExportManager(new List<IExporter> { new CsvExporter(), new PdfExporter() });
var result = exporter.TryExport(data, "text/csv", "export");
if (result.HasValue) {
    // Use result.Data, result.ContentType, result.FileName
}
```

### Authorization

```csharp
[PermissionRequirement("resource:action")]
public IActionResult MyAction() {
    // Action logic
}
```

## Configuration

### Error Handling Settings

```json
{
  "ErrorHandlingSettings": {
    "ShowErrorDetail": false
  }
}
```

## Dependencies

- CsvHelper
- MediatR
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.IdentityModel.JsonWebTokens
- Microsoft.IdentityModel.Tokens
- Serilog.AspNetCore
- Serilog.Settings.Configuration
- Serilog.Sinks.Grafana.Loki
- QuestPDF

## License

Apache-2.0