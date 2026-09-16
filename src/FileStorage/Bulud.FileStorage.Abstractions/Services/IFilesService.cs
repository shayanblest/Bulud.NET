namespace Bulud.FileStorage.Abstractions.Services;

public interface IFilesService
{
    /// <summary>
    /// Uploads content from a caller-owned stream. Implementations must leave the stream open.
    /// </summary>
    Task<string> Upload(Stream stream, long length, string contentType, string destPath, string fileName);
    Task<(Stream stream, string contentType)> Download(string filePath);
    Task Delete(string filePath);
    Task Move(string sourcePath, string destPath);
}
