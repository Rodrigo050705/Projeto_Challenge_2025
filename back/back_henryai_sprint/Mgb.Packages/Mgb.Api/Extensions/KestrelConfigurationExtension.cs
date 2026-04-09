using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Mgb.Api.Extensions;

public static class KestrelConfigurationExtension
{
    public static void ConfigureKestrelWithNetworkHelper(this WebApplicationBuilder builder)
    {
        var ip = Environment.GetEnvironmentVariable("SERVICE_ADVERTISE_HOST");
        var port = Environment.GetEnvironmentVariable("SERVICE_ADVERTISE_PORT");

        if (string.IsNullOrWhiteSpace(ip))
            throw new Exception("No local IP address found to bind Kestrel.");

        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.Listen(IPAddress.Parse(ip), int.Parse(port));
        });
    }
}