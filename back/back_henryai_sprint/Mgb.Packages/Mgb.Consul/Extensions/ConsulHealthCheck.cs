using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Mgb.Consul.Extensions;

public static class ConsulHealthCheck
{
    public static void AddConsulHealthCheck(this WebApplication application)
    {
        application.MapGet("/health", () => Results.Ok("Healthy"));
    }
}