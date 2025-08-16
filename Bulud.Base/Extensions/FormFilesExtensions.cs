using Microsoft.AspNetCore.Http;

namespace Bulud.Base.Extensions;

public static class FileExtensionHelper
{
    private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    private static readonly string[] DocumentExtensions = { ".pdf", ".doc", ".docx", ".txt" };
    private static readonly string[] AllowedExtensions = ImageExtensions.Concat(DocumentExtensions).ToArray();

    /// <summary>
    /// Gets the lowercase file extension with dot (e.g., ".jpg")
    /// </summary>
    public static string GetExtension(this IFormFile? file)
    {
        return Path.GetExtension(file?.FileName)?.ToLowerInvariant()!;
    }

    /// <summary>
    /// Validates if the file has an allowed extension
    /// </summary>
    public static bool HasAllowedExtension(this IFormFile file)
    {
        var extension = file.GetExtension();
        return !string.IsNullOrEmpty(extension) && AllowedExtensions.Contains(extension);
    }
    

    /// <summary>
    /// Validates file extension and size
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateFile(
        this IFormFile? file, 
        long maxSizeBytes = 10 * 1024 * 1024) // Default 5MB
    {
        if (file == null || file.Length == 0)
            return (false, "No file uploaded");

        var extension = file.GetExtension();

        if (string.IsNullOrEmpty(extension))
            return (false, "File has no extension");

        if (!AllowedExtensions.Contains(extension))
            return (false, $"Invalid file type. Allowed: {string.Join(", ", AllowedExtensions)}");

        if (file.Length > maxSizeBytes)
            return (false, $"File size exceeds {maxSizeBytes / (1024 * 1024)}MB limit");

        return (true, null);
    }
}