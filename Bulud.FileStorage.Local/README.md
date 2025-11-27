# Bulud.FileStorage.Local

A .NET library providing local file storage implementation for Bulud.Base IFilesService interface.

## Features

- Upload, download, delete, and move files on local disk
- Fully compatible with Bulud.Base interfaces
- Easy integration with .NET DI

## Installation

```bash
dotnet add package Bulud.FileStorage.Local
```

## Usage

### Dependency Injection Setup

```csharp
builder.Services.AddLocalFileService();
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

## File Storage Location

Files are stored in the `wwwroot` directory of your web application. The upload method creates subdirectories as needed.

## Dependencies

- Bulud.Base

## License

Apache-2.0