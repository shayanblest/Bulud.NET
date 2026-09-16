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

## Existing package warnings

The build reported existing warnings unrelated to this migration: nullable
reference warnings in `ValidationHelper`, `QueryableExtensions`, and
`FormFilesExtensions`; `NU5104` for the existing prerelease
`Serilog.Settings.Configuration` dependency; and missing package readmes for
existing packages. Dependency versions remain unchanged in this baseline; no
compatibility-driven upgrade was required.
