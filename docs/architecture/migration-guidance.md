# Modular architecture and .NET 10 migration guidance

This guide is the consumer and maintainer handoff for [issue #12](https://github.com/shayanblest/Bulud.NET/issues/12). It documents the final package layout, migration-sensitive APIs, validation commands, and intentionally preserved limitations. It does not publish packages or change provider behavior.

## Requirements

- .NET SDK 10 is required. All projects target `net10.0` through `Directory.Build.props`.
- Restore and build from the repository root:

  ```bash
  dotnet restore Bulud.NET.sln
  dotnet build Bulud.NET.sln --configuration Release --no-restore
  dotnet pack Bulud.NET.sln --configuration Release --no-build --output .artifacts/packages
  ```

The final Release build and local pack were run for this migration. Local pack produces 16 publishable packages; `Bulud.FileStorage.Azure` is intentionally excluded.

## Package map and installation

Install only the capability required by an application. Provider packages reference their matching abstraction, rather than a shared `Bulud.Base` package.

| Capability | Contracts / framework package | Provider packages |
| --- | --- | --- |
| Core | `Bulud.Core` | — |
| File storage | `Bulud.FileStorage.Abstractions` | `Bulud.FileStorage.Local`, `Bulud.FileStorage.S3` |
| Communication | `Bulud.Communication.Abstractions` | `Bulud.Communication.Email`, `Bulud.Communication.Sms.KaveNegar` |
| OTP | `Bulud.Security.Otp.Abstractions` | `Bulud.Security.Otp.InMemory` |
| Authentication | `Bulud.Authentication.Jwt` | — |
| Persistence | `Bulud.EntityFrameworkCore` | — |
| Web | `Bulud.AspNetCore` | — |
| Exporting | `Bulud.Exporting.Abstractions`, `Bulud.Exporting` | `Bulud.Exporting.Csv`, `Bulud.Exporting.Pdf` |

For example, an application using S3 file storage installs its provider:

```bash
dotnet add package Bulud.FileStorage.S3
```

The provider brings its own abstraction dependency. It does not bring communication, OTP, exporting, JWT, persistence, or web integration packages.

## Consumer migration notes

`Bulud.Base` is retired. Replace its package reference and namespaces with the package owning the API:

- primitives, common exceptions, converters, and generic extensions: `Bulud.Core`;
- file service contract: `Bulud.FileStorage.Abstractions`;
- email and SMS contracts: `Bulud.Communication.Abstractions`;
- OTP contract: `Bulud.Security.Otp.Abstractions`;
- repositories, queries, and EF/Identity infrastructure: `Bulud.EntityFrameworkCore`;
- ASP.NET Core middleware, authorization, claim/form-file helpers, and error configuration: `Bulud.AspNetCore`;
- JWT settings and registration: `Bulud.Authentication.Jwt`;
- exporting contract, orchestration, CSV, and PDF APIs: their corresponding `Bulud.Exporting*` package.

### File upload contract

`IFilesService.Upload` no longer accepts `IFormFile`. Consumers outside ASP.NET Core pass a stream, length, content type, destination path, and filename. The caller owns the stream and it remains open after the call. ASP.NET Core applications can use the form-file adapter supplied by `Bulud.AspNetCore`.

### JWT and web configuration

JWT authentication and web error handling are separate registrations. Keep the `JwtSettings` configuration key and register both features explicitly when both are used:

```csharp
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddErrorHandling(builder.Configuration);

app.UseMiddleware<GlobalExceptionMiddleware>();
```

`AddJwtAuthentication` preserves the JWT bearer defaults, validation parameters, `SaveToken = true`, and `RequireHttpsMetadata = false` from the pre-migration behavior.

### Provider registration

Registration extensions stay with their owning provider or framework package. Existing configuration keys remain `MinioSettings`, `SmsSettings`, `OtpSettings`, `JwtSettings`, and `ErrorHandlingSettings`.

## Versions and reserved packages

The migration uses `2.0.0` for existing packages moved to the new layout: `Bulud.Communication.Email`, `Bulud.Communication.Sms.KaveNegar`, `Bulud.FileStorage.Local`, `Bulud.FileStorage.S3`, and `Bulud.Security.Otp.InMemory`.

New packages begin at `1.0.0`, including Core, all abstractions, authentication, persistence, web, and exporting packages. `Bulud.FileStorage.Azure` is a reserved future provider boundary with `IsPackable=false`; it has no implementation and must not be published.

## Preserved limitations

This migration intentionally retains existing provider behavior and known limitations:

- email sending remains unimplemented;
- the Kavenegar string-message overload remains unimplemented;
- local storage download remains unimplemented;
- the existing S3 move behavior is unchanged;
- export provider selection and file-extension behavior are unchanged;
- the ASP.NET Core exception middleware retains its existing EF-specific duplicate-key handling.

These items require separate, behavior-focused issues.

## Architecture evidence for issue #12

| Parent acceptance criterion | Evidence |
| --- | --- |
| Target architecture and responsibilities are documented | `target-architecture.md` and `module-ownership.md` define the final package map and ownership for every migrated API. |
| Abstraction and provider boundaries are clear | Each capability uses a dedicated abstraction where applicable; provider project references point only to their own abstraction and required SDKs. |
| No provider depends on another provider | Project-reference inspection shows no provider-to-provider edge. |
| Framework-specific infrastructure is isolated | JWT, EF Core, and ASP.NET Core APIs are owned by `Bulud.Authentication.Jwt`, `Bulud.EntityFrameworkCore`, and `Bulud.AspNetCore`. |
| No generic framework/provider dependencies | Core has no external packages; abstraction projects have no ASP.NET Core, EF Core, DI, or provider SDK references. |
| Existing behavior is preserved | The migration tasks moved ownership mechanically and the preserved limitations above remain documented. |
| .NET 10 build succeeds | `Directory.Build.props` targets `net10.0`; the Release build command above completes successfully. |
| Documentation reflects the final structure | This guide, `README.md`, `target-architecture.md`, and `module-ownership.md` describe the final layout. |
| No unrelated functionality is included | The task changes documentation and validation evidence only; no provider behavior, CI/CD, or package publication is introduced. |
