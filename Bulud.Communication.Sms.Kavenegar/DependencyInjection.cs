using Bulud.Base.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bulud.Communication.Sms.KaveNegar;

public static class DependencyInjection
{
    public static IServiceCollection AddKavenegar(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISmsService, KavenegarService>();
        services.Configure<SmsSettings>(configuration.GetSection("SmsSettings"));

        return services;
    }
}