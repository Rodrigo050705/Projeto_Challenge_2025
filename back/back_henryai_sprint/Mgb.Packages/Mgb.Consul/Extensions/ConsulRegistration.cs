using Consul;
using Mgb.Consul.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mgb.Consul.Extensions;


public static class ConsulRegistration
{
    public static IServiceCollection AddConsulRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        var appId = configuration["AppId"];
        if (string.IsNullOrWhiteSpace(appId))
            throw new ArgumentException("AppId must be provided.", nameof(appId));

        services.AddSingleton(new ConsulConfig { AppId = appId });

        services.AddSingleton(sp =>
        {
            var addr = Environment.GetEnvironmentVariable("URL-CONSUL");
            if (string.IsNullOrWhiteSpace(addr))
                throw new InvalidOperationException("Environment variable 'URL-CONSUL' is not set.");

            return new ConsulClient(c => c.Address = new Uri(addr));
        });

        services.AddHostedService<ConsulRegistrationHostedService>();
        return services;
    }
}
