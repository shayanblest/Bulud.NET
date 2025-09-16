using Bulud.Base.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bulud.Otp;

public static class DependencyInjection
{
    public static IServiceCollection AddInMemoryOtpService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OtpSettings>(configuration.GetSection("OtpSettings"));
        services.AddScoped<IOtpService, InMemoryOtpService>();
        return services;
    }
}