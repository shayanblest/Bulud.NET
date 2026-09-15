# Target architecture

## Package map

```text
src/
  Core/Bulud.Core
  FileStorage/Bulud.FileStorage.Abstractions
  FileStorage/Bulud.FileStorage.Local
  FileStorage/Bulud.FileStorage.S3
  FileStorage/Bulud.FileStorage.Azure        (reserved, non-packable)
  Communication/Bulud.Communication.Abstractions
  Communication/Bulud.Communication.Email
  Communication/Bulud.Communication.Sms.Kavenegar
  Security/Bulud.Security.Otp.Abstractions
  Security/Bulud.Security.Otp.InMemory
  Authentication/Bulud.Authentication.Jwt
  Persistence/Bulud.EntityFrameworkCore
  Web/Bulud.AspNetCore
  Exporting/Bulud.Exporting.Abstractions
  Exporting/Bulud.Exporting
  Exporting/Bulud.Exporting.Csv
  Exporting/Bulud.Exporting.Pdf
```

## Dependency rules

- `Bulud.Core` contains only dependency-free primitives.
- A capability abstraction contains contracts and shared models only.
- A provider depends on its abstraction and any necessary external SDK; it has no provider-to-provider dependency.
- Dependency-injection registration extensions stay with their provider package.
- `Bulud.AspNetCore` owns web-host APIs, middleware, authorization helpers, HTTP/form-file helpers, and web error/logging configuration.
- `Bulud.EntityFrameworkCore` owns EF Core persistence infrastructure, repository/query helpers, and EF-specific conversions.
- `Bulud.Authentication.Jwt` owns JWT configuration and authentication integration.
- `Bulud.Exporting` owns orchestration; its abstractions and providers remain separate.

## Migration constraints

- Target `net10.0` directly.
- Preserve existing provider behavior; migration changes ownership rather than semantics.
- Existing public packages begin their breaking release at `2.0.0`; new core and abstraction packages begin at `1.0.0`.
- Upgrade third-party dependencies only when .NET 10 compatibility requires it.
- `Bulud.FileStorage.Azure` remains an architecture-only, non-packable reserved provider boundary until a dedicated implementation issue exists.
