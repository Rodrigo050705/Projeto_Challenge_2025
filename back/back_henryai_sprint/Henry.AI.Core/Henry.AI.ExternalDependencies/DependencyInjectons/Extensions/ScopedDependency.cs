using Mgb.DependencyInjections.DependencyInjectons.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mgb.DependencyInjections.DependencyInjectons.Extensions;

public static partial class DependencyInjections
{
    public static void RegisterScopedDependencies(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies(a =>
                a.GetName().Name!.StartsWith("HenryAI.", StringComparison.OrdinalIgnoreCase))

            .AddClasses(c => c.AssignableTo<IScopedDependency>())
            .As(t => t.GetInterfaces().Where(i => i != typeof(IScopedDependency)))
            .WithScopedLifetime()

        );
    }
}