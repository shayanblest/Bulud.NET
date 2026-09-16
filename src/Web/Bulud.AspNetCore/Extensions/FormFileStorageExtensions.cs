using Bulud.FileStorage.Abstractions.Services;
using Microsoft.AspNetCore.Http;

namespace Bulud.AspNetCore.Extensions;

public static class FormFileStorageExtensions
{
    public static async Task<string> Upload(
        this IFilesService filesService,
        IFormFile file,
        string destPath,
        string fileName)
    {
        await using var stream = file.OpenReadStream();
        return await filesService.Upload(stream, file.Length, file.ContentType, destPath, fileName);
    }
}
