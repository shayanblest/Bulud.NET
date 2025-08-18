using System.Web;
using Bulud.Base.Extensions;
using Bulud.Base.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Bulud.FileStorage.S3;

public class S3FileService(IMinioClient minio, IOptions<MinioSettings> settings) : IFilesService
{
    public async Task<string> Upload(IFormFile file, string destPath, string fileName)
    {
        var stream = file.OpenReadStream();
        var obj = $"{destPath.Replace('\\', '/')}/{fileName}";
        await minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket(settings.Value.Bucket)
            .WithObject(obj)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType));
        return obj;
    }

    public async Task<(Stream stream, string contentType)> Download(string filePath)
    {
        var outputStream = new MemoryStream();
        filePath = filePath.Replace("\\", "/");
        await minio.GetObjectAsync(new GetObjectArgs()
            .WithBucket(settings.Value.Bucket)
            .WithObject(HttpUtility.UrlDecode(filePath))
            .WithCallbackStream(stream => { stream.CopyTo(outputStream); }));

        outputStream.Position = 0;
        var contentType = filePath.GetMimeType();

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
}

public class MinioSettings
{
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public required string Bucket { get; set; }
    public bool UseSSL { get; set; }
}