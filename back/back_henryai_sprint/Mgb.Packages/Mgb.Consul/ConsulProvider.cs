using Consul;

namespace Mgb.Consul;

public static class ConsulProvider
{
    private static readonly ConsulClient _client = new ConsulClient(c =>
    {
        // Read from environment variable
        var consulAddress = Environment.GetEnvironmentVariable("URL-CONSUL");

        if (string.IsNullOrWhiteSpace(consulAddress))
        {
            throw new InvalidOperationException("Environment variable 'URL-CONSUL' is not set.");
        }

        c.Address = new Uri(consulAddress);
    });

    public static ConsulClient Client => _client;
}