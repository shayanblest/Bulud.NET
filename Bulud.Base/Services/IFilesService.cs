using Microsoft.AspNetCore.Http;

namespace Bulud.Base.Services;

public interface IFilesService
{
    Task<string> Upload(IFormFile file, string destPath, string fileName);
    Task<(Stream stream, string contentType)> Download(string filePath);
    Task Delete(string filePath);
    Task Move(string sourcePath, string destPath);
}