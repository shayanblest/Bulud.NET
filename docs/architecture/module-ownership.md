# Module ownership inventory

This is the implementation inventory for [issue #12](https://github.com/shayanblest/Bulud.NET/issues/12). It assigns every source API and direct dependency in the current solution to one target package. It describes ownership only: the migration tasks must preserve the listed behavior unless their acceptance criteria explicitly require a signature or registration change.

## Target ownership

| Target owner | Current source APIs and helpers | Required migration change |
| --- | --- | --- |
| `Bulud.Core` | `Entities/BaseEntity`; `Converters/UtcDateTimeConverter`; `Exceptions/DateExtensions`, `DuplicateRequestException`, `ForbiddenException`, `NotFoundException`; `Extensions/CollectionExtensions`, `DateExtensions`, `ExpressionExtensions`, and `ValidationHelper` | Move dependency-free primitives under a `Bulud.Core` namespace. `UtcDateTimeConverter` is a `System.Text.Json` converter and belongs here, **not** in `Bulud.EntityFrameworkCore`. Keep both existing date helpers distinct; they currently have different namespaces. |
| `Bulud.FileStorage.Abstractions` | `Services/IFilesService` | Replace the ASP.NET-bound `Upload(IFormFile, string, string)` contract with a stream, explicit length, content type, destination path, and filename. Keep download, delete, and move contracts. The abstraction must have no ASP.NET dependency. |
| `Bulud.FileStorage.Local` | `Bulud.FileStorage.Local/LocalFileService`; `DependencyInfection.AddLocalFileService` | Implement the new storage contract while retaining current paths and return values. Remove the `IWebHostEnvironment` requirement from the provider; the present upload implementation already writes from `Directory.GetCurrentDirectory()/wwwroot`. Preserve the current unimplemented `Download` operation. |
| `Bulud.FileStorage.S3` | `Bulud.FileStorage.S3/S3FileService`, `MinioSettings`, and `DependencyInjection.AddS3FileService` | Implement the new storage contract without closing a caller-owned stream. Continue registering Minio and binding `MinioSettings` from `MinioSettings`; remove direct use of `IFormFile` and the web MIME helper. |
| `Bulud.FileStorage.Azure` | `Bulud.FileStorage.Azure/Class1` | Retire the placeholder `Class1`; retain this package only as the reserved, unimplemented, non-packable Azure boundary. No Azure provider is implemented by this migration. |
| `Bulud.Communication.Abstractions` | `Services/IEmailService`, `Services/ISmsService` | Move contracts and preserve their method signatures. No provider or DI dependency belongs here. |
| `Bulud.Communication.Email` | `Bulud.Communication.Email/EmailService` and `DependencyInjection.AddEmail` | Depend only on communication abstractions. Preserve the current `NotImplementedException` from `SendAsync`; no email provider behavior is added. Keep the `AddEmail` registration extension with the provider. |
| `Bulud.Communication.Sms.Kavenegar` | `Bulud.Communication.Sms.KaveNegar/KaveNegarService`, `SmsSettings`, and `DependencyInjection.AddKaveNegar` | Depend only on communication abstractions and `Microsoft.Extensions.Options`. Preserve the `SmsSettings` configuration key and registration. Preserve the current token lookup request and the unimplemented string-message overload. |
| `Bulud.Security.Otp.Abstractions` | `Services/IOtpService` | Move the contract without changing generation or verification semantics. |
| `Bulud.Security.Otp.InMemory` | `Bulud.Security.InMemoryOtp/InMemoryOtpService`, `OtpSettings`, and `DependencyInjection.AddInMemoryOtpService` | Depend only on the OTP abstraction plus caching/options. Preserve `OtpSettings`, cache key format, expiration, code generation, single-use verification, and scoped registration. |
| `Bulud.EntityFrameworkCore` | `Infrastructure/AppDbContextBase`, `IRepository`, `QueryableExtensions`; `Queries/RequestQuery`, `ListRequestQuery`; `ListResult` | Move repository/query types and EF/Identity infrastructure together. Preserve defaults, nullability, filtering, sorting, includes, paging helpers, and MediatR usage. `MediatR` stays here, outside Core. |
| `Bulud.AspNetCore` | `Attributes/PermissionRequirementAttribute`; `Authorization/PermissionRequirement`; `Exceptions/AppValidationException`; `Extensions/ClaimPrincipalExtensions`, `FormFilesExtensions`, `LogContextExtensions`, `MimeTypeExtensions`; `Middlewares/GlobalExceptionMiddleware`; `settingOptions/ErrorHandlingSettings` | Own MVC/authorization, HTTP/form-file, static-file MIME, middleware, logging, and web error configuration. Add the `IFormFile`-to-storage-contract adapter here. `ErrorHandlingSettings` must receive explicit web registration rather than its current incidental registration by JWT setup. |
| `Bulud.Authentication.Jwt` | root `DependencyInjection.AddJwtAuthentication`; `settingOptions/JwtSettings` | Move JWT settings and registration together. Preserve the `JwtSettings` key, schemes, validation parameters, `SaveToken`, and `RequireHttpsMetadata` value. Remove its implicit configuration of `ErrorHandlingSettings`. |
| `Bulud.Exporting.Abstractions` | `Exporters/IExporter` | Move the exporter contract without changing its generic export shape. |
| `Bulud.Exporting` | `Exporters/ExportManager` | Move exporter orchestration and preserve provider selection, return tuple, content type, and file-name behavior. |
| `Bulud.Exporting.Csv` | `Exporters/CsvExporter` | Move the CSV implementation and its CSV dependency. Preserve output and current extension value. |
| `Bulud.Exporting.Pdf` | `Exporters/PdfExporter` | Move the PDF implementation and its PDF dependency. Preserve generated document behavior. |

## Source coverage

Every hand-written C# source file in the current solution is accounted for below. The project files are covered in the dependency inventory.

| Current location | Target owner |
| --- | --- |
| `Bulud.Base/Attributes/PermissionRequirementAttribute.cs`, `Bulud.Base/Authorization/PermissionRequirement.cs` | `Bulud.AspNetCore` |
| `Bulud.Base/Converters/UtcDateTimeConverter.cs`, `Bulud.Base/Entities/BaseEntity.cs` | `Bulud.Core` |
| `Bulud.Base/DependencyInjection.cs`, `Bulud.Base/settingOptions/JwtSettings.cs` | `Bulud.Authentication.Jwt` |
| `Bulud.Base/Exceptions/AppValidationException.cs` | `Bulud.AspNetCore` |
| `Bulud.Base/Exceptions/DateExtensions.cs`, `Bulud.Base/Exceptions/DuplicateRequestException.cs`, `Bulud.Base/Exceptions/ForbiddenException.cs`, `Bulud.Base/Exceptions/NotFoundException.cs` | `Bulud.Core` |
| `Bulud.Base/Exporters/IExporter.cs` | `Bulud.Exporting.Abstractions` |
| `Bulud.Base/Exporters/ExportManager.cs` | `Bulud.Exporting` |
| `Bulud.Base/Exporters/CsvExporter.cs` | `Bulud.Exporting.Csv` |
| `Bulud.Base/Exporters/PdfExporter.cs` | `Bulud.Exporting.Pdf` |
| `Bulud.Base/Extensions/ClaimPrincipalExtensions.cs`, `Bulud.Base/Extensions/FormFilesExtensions.cs`, `Bulud.Base/Extensions/LogContextExtensions.cs`, `Bulud.Base/Extensions/MimeTypeExtensions.cs` | `Bulud.AspNetCore` |
| `Bulud.Base/Extensions/CollectionExtensions.cs`, `Bulud.Base/Extensions/DateExtensions.cs`, `Bulud.Base/Extensions/ExpressionExtensions.cs`, `Bulud.Base/Extensions/ValidationHelper.cs` | `Bulud.Core` |
| `Bulud.Base/Infrastructure/AppDbContextBase.cs`, `Bulud.Base/Infrastructure/IRepository.cs`, `Bulud.Base/Infrastructure/QueryableExtensions.cs` | `Bulud.EntityFrameworkCore` |
| `Bulud.Base/ListResult.cs`, `Bulud.Base/Queries/ListRequestQuery.cs`, `Bulud.Base/Queries/RequestQuery.cs` | `Bulud.EntityFrameworkCore` |
| `Bulud.Base/Middlewares/GlobalExceptionMiddleware.cs`, `Bulud.Base/settingOptions/ErrorHandlingSettings.cs` | `Bulud.AspNetCore` |
| `Bulud.Base/Services/IEmailService.cs`, `Bulud.Base/Services/ISmsService.cs` | `Bulud.Communication.Abstractions` |
| `Bulud.Base/Services/IFilesService.cs` | `Bulud.FileStorage.Abstractions` |
| `Bulud.Base/Services/IOtpService.cs` | `Bulud.Security.Otp.Abstractions` |
| `Bulud.Communication.Email/DependencyInjection.cs`, `Bulud.Communication.Email/EmailService.cs` | `Bulud.Communication.Email` |
| `Bulud.Communication.Sms.KaveNegar/DependencyInjection.cs`, `Bulud.Communication.Sms.KaveNegar/KaveNegarService.cs`, `Bulud.Communication.Sms.KaveNegar/SmsSettings.cs` | `Bulud.Communication.Sms.Kavenegar` |
| `Bulud.FileStorage.Local/DependencyInfection.cs`, `Bulud.FileStorage.Local/LocalFileService.cs` | `Bulud.FileStorage.Local` |
| `Bulud.FileStorage.S3/DependencyInjection.cs`, `Bulud.FileStorage.S3/S3FileService.cs` | `Bulud.FileStorage.S3` |
| `Bulud.FileStorage.Azure/Class1.cs` | `Bulud.FileStorage.Azure` (reserved only) |
| `Bulud.Security.InMemoryOtp/DependencyInjection.cs`, `Bulud.Security.InMemoryOtp/InMemoryOtpService.cs` | `Bulud.Security.Otp.InMemory` |

## Dependency inventory and destination

Project-file coverage: `Bulud.Base/Bulud.Base.csproj` is the current shared package; `Bulud.Communication.Email/Bulud.Communication.Email.csproj`, `Bulud.Communication.Sms.KaveNegar/Bulud.Communication.Sms.Kavenegar.csproj`, `Bulud.FileStorage.Local/Bulud.FileStorage.Local.csproj`, `Bulud.FileStorage.S3/Bulud.FileStorage.S3.csproj`, and `Bulud.Security.InMemoryOtp/Bulud.Security.InMemoryOtp.csproj` are existing provider projects; `Bulud.FileStorage.Azure/Bulud.FileStorage.Azure.csproj` is the reserved Azure placeholder. All target `net8.0` today and are assigned below for the direct `net10.0` migration.

| Current direct dependency | Current owner | Target owner | Reason and boundary |
| --- | --- | --- | --- |
| `CsvHelper` 33.1.0 | `Bulud.Base` | `Bulud.Exporting.Csv` | CSV provider implementation only. |
| `QuestPDF` 2025.7.0 | `Bulud.Base` | `Bulud.Exporting.Pdf` | PDF provider implementation only. |
| `MediatR` 12.4.1 | `Bulud.Base` | `Bulud.EntityFrameworkCore` | Used by `ListRequestQuery`; it is not a Core dependency. |
| `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.12 | `Bulud.Base` | `Bulud.Authentication.Jwt` | JWT registration and bearer options. |
| `Microsoft.IdentityModel.JsonWebTokens` 8.4.0 and `Microsoft.IdentityModel.Tokens` 8.4.0 | `Bulud.Base` | `Bulud.Authentication.Jwt` | JWT token validation. |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 8.0.12 | `Bulud.Base` | `Bulud.EntityFrameworkCore` | `AppDbContextBase` and Identity persistence infrastructure. |
| `Serilog.AspNetCore` 9.0.0, `Serilog.Settings.Configuration` 9.0.1-dev-02317, and `Serilog.Sinks.Grafana.Loki` 8.3.1 | `Bulud.Base` | `Bulud.AspNetCore` | HTTP request logging and web error middleware. The inventory records the existing package set; compatibility upgrades belong to T2 only if required for .NET 10. |
| `Minio` 6.0.5 | `Bulud.FileStorage.S3` | `Bulud.FileStorage.S3` | S3 provider SDK only. |
| `Microsoft.Extensions.Options` 9.0.0 | `Bulud.Communication.Sms.KaveNegar` | `Bulud.Communication.Sms.Kavenegar` | Options binding for `SmsSettings`; no communication-abstraction dependency. |
| `Microsoft.Extensions.Options` 9.0.0 | `Bulud.Security.InMemoryOtp` | `Bulud.Security.Otp.InMemory` | Options binding for `OtpSettings`; no OTP-abstraction dependency. |
| `Microsoft.Extensions.Caching.Abstractions` 8.0.0 | `Bulud.Security.InMemoryOtp` | `Bulud.Security.Otp.InMemory` | In-memory OTP cache implementation only. |
| `ProjectReference` from `Bulud.Communication.Email` to `Bulud.Base` | `Bulud.Communication.Email` | `Bulud.Communication.Abstractions` | The email provider uses only `IEmailService`; it must not retain a reference to the former shared package. |
| `ProjectReference` from `Bulud.Communication.Sms.KaveNegar` to `Bulud.Base` | `Bulud.Communication.Sms.KaveNegar` | `Bulud.Communication.Abstractions` | The Kavenegar provider uses only `ISmsService` plus its own options dependency. |
| `ProjectReference` from `Bulud.FileStorage.Local` to `Bulud.Base` | `Bulud.FileStorage.Local` | `Bulud.FileStorage.Abstractions` | The Local provider uses only `IFilesService`; its web-host dependency is removed by T6. |
| `ProjectReference` from `Bulud.FileStorage.S3` to `Bulud.Base` | `Bulud.FileStorage.S3` | `Bulud.FileStorage.Abstractions` | The S3 provider uses only `IFilesService` plus Minio; its form-file and web MIME dependencies are removed by T6. |
| `ProjectReference` from `Bulud.Security.InMemoryOtp` to `Bulud.Base` | `Bulud.Security.InMemoryOtp` | `Bulud.Security.Otp.Abstractions` | The in-memory provider uses only `IOtpService` plus caching and options. |
| ASP.NET Core HTTP, MVC, authorization, hosting, static-files, configuration, DI, and logging APIs currently supplied by the `Bulud.Base` web SDK | `Bulud.Base` | `Bulud.AspNetCore` | Web-host APIs move together; Core, abstractions, and providers do not retain these framework dependencies. |
| EF Core APIs currently supplied by the `Bulud.Base` web SDK | `Bulud.Base` | `Bulud.EntityFrameworkCore` | EF infrastructure, repository/query helpers, and their EF framework dependency move together. |

## Required signature and registration changes

- T6 changes `IFilesService.Upload` from an `IFormFile` input to caller-supplied stream, length, and content type. `Bulud.AspNetCore` supplies the form-file adapter; Local and S3 must not reference ASP.NET form-file types. The migration must state stream ownership explicitly and leave caller-owned streams open.
- T6 also removes Local storage's `IWebHostEnvironment` dependency. MIME lookup is presently provided by `MimeTypeExtensions` through `Microsoft.AspNetCore.StaticFiles`; it is owned by the Web package. S3 download currently uses it, so T6 needs a compatible provider-local or abstraction-safe content-type strategy without a dependency on `Bulud.AspNetCore`.
- T9 keeps `PermissionRequirementAttribute`, claims accessors, form-file validation, middleware, and logging as Web APIs. Its explicit Web registration must bind `ErrorHandlingSettings`.
- T10 keeps `AddJwtAuthentication` and `JwtSettings` together. It must stop configuring `ErrorHandlingSettings` as a side effect, while retaining the present JWT configuration keys and values.
- T4, T5, T6, and T7 move registration extensions with their providers/orchestrator and replace their `Bulud.Base` references with only their assigned abstraction and required SDK/framework dependencies.

## Preserved incomplete operations and defects

These are baseline observations, not authorization to repair them in issue #12:

- `EmailService.SendAsync` throws `NotImplementedException`.
- `KaveNegarService.SendAsync(string, string)` throws `NotImplementedException`; the token overload performs the existing lookup request.
- `LocalFileService.Download` throws `NotImplementedException`.
- `S3FileService.Move` copies from source to destination, then removes the destination rather than the source. This is an existing behavioral defect and must not be silently changed during the package move.
- `ExportManager.TryExport` selects by exact `ContentType`, even though providers expose `CanHandle`; CSV's extension is `"csv"` while PDF's is `".pdf"`. Those observable file-selection/naming behaviors must be preserved unless a separate issue authorizes a fix.
- `GlobalExceptionMiddleware` contains the existing TODO to refactor exception handling and has EF-specific duplicate-key handling. It remains Web-owned; the migration does not redesign it.
- `Bulud.FileStorage.Azure` contains only the empty `Class1` placeholder and has no provider operation to preserve.

## Boundary checks for subsequent tasks

- `Bulud.Core` has no external package or framework reference.
- Every abstraction package contains contracts/shared models only and has no provider SDK, DI, ASP.NET, or EF reference.
- Each provider references only its own abstraction plus needed SDK/framework packages; providers never reference each other.
- `Bulud.EntityFrameworkCore`, `Bulud.AspNetCore`, and `Bulud.Authentication.Jwt` own their framework-specific APIs.
- The target framework migration is directly to `net10.0`; version and compatibility changes are recorded by T2.
