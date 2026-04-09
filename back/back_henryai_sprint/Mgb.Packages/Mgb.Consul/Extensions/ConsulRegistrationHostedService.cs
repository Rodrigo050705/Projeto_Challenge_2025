using Consul;
using Mgb.Consul.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mgb.Consul.Extensions;

internal sealed class ConsulRegistrationHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IServiceProvider _services;
    private readonly ConsulClient _consul;
    private readonly ConsulConfig _cfg;
    private readonly ILogger<ConsulRegistrationHostedService> _logger;
    private readonly IConfiguration _configuration;

    private string? _serviceId;

    public ConsulRegistrationHostedService(
        IHostApplicationLifetime lifetime,
        IServiceProvider services,
        ConsulClient consul,
        ConsulConfig cfg,
        ILogger<ConsulRegistrationHostedService> logger, 
        IConfiguration configuration)
    {
        _lifetime = lifetime;
        _services = services;
        _consul = consul;
        _cfg = cfg;
        _logger = logger;
        _configuration = configuration;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Register once the app is fully started so Kestrel has bound real addresses/ports
        _lifetime.ApplicationStarted.Register(() =>
        {
            _ = Task.Run(RegisterAsync); // fire and forget inside Start
        });

        // Ensure we deregister on graceful shutdown
        _lifetime.ApplicationStopping.Register(() => { _ = Task.Run(DeregisterAsync); });
        _lifetime.ApplicationStopped.Register(() => { _ = Task.Run(DeregisterAsync); });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task RegisterAsync()
    {
        try
        {
            var (host, port, url) = GetPrimaryHttpEndpoint();
            _serviceId = _cfg.AppId; // stable and provided by the consumer

            // Health check tuning (friendlier in DEBUG to tolerate breakpoints)
#if DEBUG
            var interval = TimeSpan.FromSeconds(10);
            var timeout = TimeSpan.FromSeconds(10);
            var deregAfter = TimeSpan.FromMinutes(25); // don't nuke the service quickly during debug pauses
#else
                var interval = TimeSpan.FromSeconds(5);
                var timeout = TimeSpan.FromSeconds(2);
                var deregAfter = TimeSpan.FromSeconds(10);
#endif
            var fabioTag = _configuration["RoutePrefix"];
            var registration = new AgentServiceRegistration
            {
                ID = _serviceId,
                Name = _cfg.AppId,
                Address = host, // the host from Kestrel
                Port = port, // the actual bound port
                Check = new AgentServiceCheck
                {
                    HTTP = $"{url.TrimEnd('/')}/health",
                    Interval = interval,
                    Timeout = timeout,
                    // Backstop for crashes; we still explicitly deregister on shutdown
                    DeregisterCriticalServiceAfter = deregAfter
                },Tags = [fabioTag]
            };

            await _consul.Agent.ServiceRegister(registration);
            _logger.LogInformation("Consul registered {ServiceId} at {Url}", _serviceId, url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Consul registration failed for {AppId}", _cfg.AppId);
        }
    }

    private async Task DeregisterAsync()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(_serviceId))
            {
                await _consul.Agent.ServiceDeregister(_serviceId);
                _logger.LogInformation("Consul deregistered {ServiceId}", _serviceId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Consul deregistration failed for {ServiceId}", _serviceId);
        }
    }

   private (string host, int port, string url) GetPrimaryHttpEndpoint()
{
    var envHost = Environment.GetEnvironmentVariable("SERVICE_ADVERTISE_HOST");
    var envPort = Environment.GetEnvironmentVariable("SERVICE_ADVERTISE_PORT");
    var url = $"http://{envHost}:{envPort}";
    return (envHost, int.Parse(envPort), url);
}
}