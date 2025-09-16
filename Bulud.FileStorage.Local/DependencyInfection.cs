using Bulud.Base.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bulud.FileStorage.Local;

public static class DependencyInjection
{
    public static IServiceCollection AddLocalFileService(this IServiceCollection services)
    {
        services.AddScoped<IFilesService ,LocalFileService>();
        return services;
    }
}