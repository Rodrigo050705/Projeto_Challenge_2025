using Mgb.DependencyInjections.DependencyInjectons.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mgb.DependencyInjections.DependencyInjectons.Extensions;

public static partial class DependencyInjections
{
    public static IServiceCollection RegisterAllDependencies(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies(a =>
                !a.GetName().Name!.StartsWith("System", StringComparison.Ordinal) &&
                !a.GetName().Name!.StartsWith("Microsoft", StringComparison.Ordinal))

            // transient
            .AddClasses(c => c.AssignableTo<ITransientDependency>())
            .As(t => t.GetInterfaces().Where(i => i != typeof(ITransientDependency)))
            .WithTransientLifetime()

            // scoped
            .AddClasses(c => c.AssignableTo<IScopedDependency>())
            .As(t => t.GetInterfaces().Where(i => i != typeof(IScopedDependency)))
            .WithScopedLifetime()

            // singleton
            .AddClasses(c => c.AssignableTo<ISingletonDependency>())
            .As(t => t.GetInterfaces().Where(i => i != typeof(ISingletonDependency)))
            .WithSingletonLifetime()
        );

        return services;
    }
}