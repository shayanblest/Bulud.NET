# Bulud.Base

A comprehensive utility library for .NET applications providing essential infrastructure components, exception handling, extensions, and more.

## Features

- **Custom Exception Handling**: Pre-built exceptions for common scenarios (Validation, Forbidden, NotFound, DuplicateRequest)
- **Global Exception Middleware**: Integrated with Serilog and Loki for logging and monitoring
- **Extension Methods**: Utilities for EF Core, Claims, Collections, Dates, and more
- **Base Infrastructure**: Repository pattern and DbContext base classes
- **Authorization Helpers**: Permission-based authorization helpers
- **JSON Converters**: UTC DateTime converter for consistent serialization

## Installation

```bash
dotnet add package Bulud.Base
```

## Usage

### Package registration

JWT authentication is provided by `Bulud.Authentication.Jwt`, while web error
handling is provided by `Bulud.AspNetCore`. See each package's README for the
corresponding registration call.

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

- MediatR
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Serilog.AspNetCore
- Serilog.Settings.Configuration
- Serilog.Sinks.Grafana.Loki

## License

Apache-2.0
