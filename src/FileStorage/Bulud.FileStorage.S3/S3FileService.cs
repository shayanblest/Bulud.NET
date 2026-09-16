using System.Net;
using Bulud.FileStorage.Abstractions.Services;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Bulud.FileStorage.S3;

public class S3FileService(IMinioClient minio, IOptions<MinioSettings> settings) : IFilesService
{
    public async Task<string> Upload(Stream stream, long length, string contentType, string destPath, string fileName)
    {
        var obj = $"{destPath.Replace('\\', '/')}/{fileName}";
        await minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket(settings.Value.Bucket)
            .WithObject(obj)
            .WithStreamData(stream)
            .WithObjectSize(length)
            .WithContentType(contentType));
        return obj;
    }

    public async Task<(Stream stream, string contentType)> Download(string filePath)
    {
        var outputStream = new MemoryStream();
        filePath = filePath.Replace("\\", "/");
        await minio.GetObjectAsync(new GetObjectArgs()
            .WithBucket(settings.Value.Bucket)
            .WithObject(WebUtility.UrlDecode(filePath))
            .WithCallbackStream(stream => { stream.CopyTo(outputStream); }));

        outputStream.Position = 0;
        var contentType = GetContentType(filePath);

        return (outputStream, contentType);
    }


    public async Task Delete(string filePath)
    {
        filePath = filePath.Replace("\\", "/");
        await minio.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(settings.Value.Bucket)
            .WithObject(filePath));
    }

    public async Task Move(string sourcePath, string destPath)
    {
        sourcePath = sourcePath.Replace("\\", "/");
        destPath = destPath.Replace("\\", "/");
        var copySource = new CopySourceObjectArgs()
            .WithBucket(settings.Value.Bucket)
            .WithObject(sourcePath);

        await minio.CopyObjectAsync(new CopyObjectArgs()
            .WithBucket(settings.Value.Bucket)
            .WithObject(destPath)
            .WithCopyObjectSource(copySource));

        await minio.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(settings.Value.Bucket)
            .WithObject(destPath));
    }

    private static string GetContentType(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".3gp" => "video/3gpp",
            ".3g2" or ".3gp2" => "video/3gpp2",
            ".aac" => "audio/aac",
            ".aif" => "audio/x-aiff",
            ".aiff" or ".aifc" => "audio/aiff",
            ".avi" => "video/x-msvideo",
            ".avif" => "image/avif",
            ".bmp" => "image/bmp",
            ".css" => "text/css",
            ".csv" => "text/csv",
            ".dib" => "image/bmp",
            ".doc" => "application/msword",
            ".docm" => "application/vnd.ms-word.document.macroEnabled.12",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".eot" => "application/vnd.ms-fontobject",
            ".flv" => "video/x-flv",
            ".gif" => "image/gif",
            ".gz" => "application/x-gzip",
            ".htm" or ".html" => "text/html",
            ".ico" => "image/x-icon",
            ".jar" => "application/java-archive",
            ".jpeg" or ".jpg" => "image/jpeg",
            ".js" or ".mjs" => "text/javascript",
            ".json" => "application/json",
            ".m4a" => "audio/mp4",
            ".m4v" or ".mp4v" => "video/mp4",
            ".mid" or ".midi" or ".rmi" => "audio/mid",
            ".mov" or ".qt" => "video/quicktime",
            ".mp2" or ".mpa" or ".mpe" or ".mpeg" or ".mpg" => "video/mpeg",
            ".mp3" => "audio/mpeg",
            ".mp4" => "video/mp4",
            ".oga" => "audio/ogg",
            ".ogg" or ".ogv" => "video/ogg",
            ".ogx" => "application/ogg",
            ".otf" => "font/otf",
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".ppt" or ".pps" or ".pot" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".rar" => "application/octet-stream",
            ".rtf" => "application/rtf",
            ".svg" => "image/svg+xml",
            ".svgz" => "image/svg+xml",
            ".tar" => "application/x-tar",
            ".tif" or ".tiff" => "image/tiff",
            ".ts" => "video/vnd.dlna.mpeg-tts",
            ".tsv" => "text/tab-separated-values",
            ".ttf" or ".ttc" => "application/x-font-ttf",
            ".txt" => "text/plain",
            ".wav" => "audio/wav",
            ".webm" => "video/webm",
            ".webp" => "image/webp",
            ".woff" => "application/font-woff",
            ".woff2" => "font/woff2",
            ".xls" => "application/vnd.ms-excel",
            ".xlsb" => "application/vnd.ms-excel.sheet.binary.macroEnabled.12",
            ".xlsm" => "application/vnd.ms-excel.sheet.macroEnabled.12",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xml" or ".xsd" or ".xsl" or ".xslt" => "text/xml",
            ".zip" => "application/x-zip-compressed",
            _ => "application/octet-stream"
        };
    }
}

public class MinioSettings
{
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public required string Bucket { get; set; }
    public bool UseSSL { get; set; }
}
