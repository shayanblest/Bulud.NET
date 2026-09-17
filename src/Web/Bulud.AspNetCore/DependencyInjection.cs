using Bulud.AspNetCore.settingOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bulud.AspNetCore;

public static class DependencyInjection
{
    public static IServiceCollection AddErrorHandling(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ErrorHandlingSettings>(configuration.GetSection("ErrorHandlingSettings"));
        return services;
    }
}
