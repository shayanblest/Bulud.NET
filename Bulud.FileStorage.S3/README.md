# Bulud.FileStorage.S3

A .NET library providing S3 file storage implementation for Bulud.Base IFilesService interface.

## Features

- Upload, download, delete, and move files in S3
- Fully compatible with Bulud.Base interfaces
- Easy integration with .NET DI

## Installation

```bash
dotnet add package Bulud.FileStorage.S3
```

## Usage

### Configuration

Add MinIO/S3 settings to your `appsettings.json`:

```json
{
  "MinioSettings": {
    "Endpoint": "localhost:9000",
    "AccessKey": "your-access-key",
    "SecretKey": "your-secret-key",
    "Bucket": "your-bucket-name",
    "UseSSL": false
  }
}
```

### Dependency Injection Setup

```csharp
builder.Services.AddS3FileService(builder.Configuration);
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
- Minio

## License

Apache-2.0