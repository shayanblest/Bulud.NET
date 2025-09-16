using Bulud.Base.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace Bulud.FileStorage.S3;

public static class DependencyInjection
{
    public static IServiceCollection AddS3FileService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMinioClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MinioSettings>>().Value;

            return new MinioClient()
                .WithEndpoint(settings.Endpoint)
                .WithCredentials(settings.AccessKey, settings.SecretKey)
                .WithSSL(settings.UseSSL)
                .Build();
        });
        
        services.Configure<MinioSettings>(configuration.GetSection("MinioSettings"));
        services.AddScoped<IFilesService ,S3FileService>();
        return services;
    }
}