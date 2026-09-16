using Bulud.FileStorage.Abstractions.Services;

namespace Bulud.FileStorage.Local;

public class LocalFileService : IFilesService
{
    public async Task<string> Upload(Stream stream, long length, string contentType, string destPath, string fileName)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", destPath);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var filePath = Path.Combine(path, fileName);

        await using var outputStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(outputStream);

        return $"/{destPath}/{fileName}";
    }

    public Task<(Stream stream, string contentType)> Download(string filePath)
    {
        throw new NotImplementedException();
    }

    public Task Delete(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }

    public Task Move(string sourcePath, string destPath)
    {
        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var sPath = Path.Combine(webRootPath, sourcePath.TrimStart('/'));
        var dPath = Path.Combine(webRootPath, destPath.TrimStart('/'));
        File.Move(sPath, dPath);
        return Task.CompletedTask;
    }
}
