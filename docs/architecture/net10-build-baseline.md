# .NET 10 build baseline

## SDK

Validation used the official .NET SDK `10.0.401` on Windows x64. A local SDK
was used because the system drive did not have enough space for a system-wide
installation; it was removed after validation and is not part of this
repository.

## Commands and results

```powershell
.\.dotnet\dotnet.exe restore Bulud.NET.sln
.\.dotnet\dotnet.exe build Bulud.NET.sln --configuration Release --no-restore --disable-build-servers -m:1 -nr:false
```

Restore completed for all seven projects. The Release build completed with exit
code `0` and zero errors.

## Build warning follow-up

The initial baseline reported nullable reference warnings in `ValidationHelper`,
`QueryableExtensions`, and `FormFilesExtensions`, plus `NU5104` for a prerelease
`Serilog.Settings.Configuration` dependency. Those warnings were resolved by
adding null checks, correcting nullable annotations, checking reflection
results, and moving to the stable `9.0.0` package. The current Release build
completes with zero warnings and zero errors. NuGet emits informational messages
for packable projects that do not include a package readme.
