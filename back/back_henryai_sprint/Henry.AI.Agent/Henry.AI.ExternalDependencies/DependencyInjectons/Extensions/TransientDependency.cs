using Mgb.DependencyInjections.DependencyInjectons.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mgb.DependencyInjections.DependencyInjectons.Extensions;

public static partial class DependencyInjections
{
    public static void RegisterTransientDependencies(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies(a =>
                a.GetName().Name!.StartsWith("HenryAI.", StringComparison.OrdinalIgnoreCase))

            .AddClasses(c => c.AssignableTo<ITransientDependency>())
            .As(t => t.GetInterfaces().Where(i => i != typeof(ITransientDependency)))
            .WithTransientLifetime()

        );

    }
}