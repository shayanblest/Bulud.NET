using Bulud.Base.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Bulud.FileStorage.Local;

public class LocalFileService(IWebHostEnvironment env) : IFilesService
{
    public async Task<string> Upload(IFormFile file, string destPath, string fileName)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", destPath);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var filePath = Path.Combine(path, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

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
        var sPath = Path.Combine(env.WebRootPath, sourcePath.TrimStart('/'));
        var dPath = Path.Combine(env.WebRootPath, destPath.TrimStart('/'));
        File.Move(sPath, dPath);
        return Task.CompletedTask;
    }
}