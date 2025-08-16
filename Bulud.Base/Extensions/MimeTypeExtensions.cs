using Microsoft.AspNetCore.StaticFiles;

namespace Bulud.Base.Extensions;

public static class MimeTypeExtensions
{
    private static readonly FileExtensionContentTypeProvider Provider = new();

    public static string GetMimeType(this string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return "application/octet-stream";

        if (Provider.TryGetContentType(fileName, out var contentType))
            return contentType;

        return "application/octet-stream";
    }
}