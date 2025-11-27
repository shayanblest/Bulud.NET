# Bulud.FileStorage.Azure

A .NET library providing Azure Blob Storage implementation for Bulud.Base IFilesService interface.

## Features

- Upload, download, delete, and move files in Azure Blob Storage
- Fully compatible with Bulud.Base interfaces
- Easy integration with .NET DI

## Installation

```bash
dotnet add package Bulud.FileStorage.Azure
```

## Usage

### Configuration

Add Azure settings to your `appsettings.json`:

```json
{
  "AzureSettings": {
    "ConnectionString": "your-azure-connection-string",
    "ContainerName": "your-container-name"
  }
}
```

### Dependency Injection Setup

```csharp
builder.Services.AddAzureFileService(builder.Configuration);
```

### File Operations

```csharp
public class MyService
{
    private readonly IFilesService _fileService;

    public MyService(IFilesService fileService)
    {
        _fileService = fileService;
    }

    public async Task<string> UploadFile(IFormFile file)
    {
        return await _fileService.Upload(file, "uploads", "myfile.jpg");
    }

    public async Task<(Stream stream, string contentType)> DownloadFile(string filePath)
    {
        return await _fileService.Download(filePath);
    }

    public async Task DeleteFile(string filePath)
    {
        await _fileService.Delete(filePath);
    }

    public async Task MoveFile(string sourcePath, string destPath)
    {
        await _fileService.Move(sourcePath, destPath);
    }
}
```

## Dependencies

- Bulud.Base
- Azure.Storage.Blobs

## License

Apache-2.0