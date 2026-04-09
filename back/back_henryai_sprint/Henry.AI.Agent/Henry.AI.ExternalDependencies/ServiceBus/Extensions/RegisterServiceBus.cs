using System.Reflection;
using Mgb.ServiceBus.ServiceBus.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Messages;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;

namespace Mgb.ServiceBus.ServiceBus.Extensions;

public static class ServiceBus
{
    public static IServiceCollection RegisterServiceBus(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rabbitMqConnString = configuration["ServiceBus:ConnectionString"];
        var inputQueueName     = configuration["AppId"];
        
        services.Scan(scan => scan
            .FromApplicationDependencies(assembly =>
                !IsFrameworkAssembly(assembly.GetName().Name))
            .AddClasses(c => c.AssignableTo(typeof(IHandleMessages<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime()
        );

        var assemblies = LoadAllAppAssemblies();

        var messageRoutes = assemblies
            .SelectMany(SafeGetTypes)
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .Where(t => typeof(ICommand).IsAssignableFrom(t))
            .Select(t => new { Type = t, Attr = t.GetCustomAttribute<CommandAttribute>() })
            .Where(x => x.Attr != null && !string.IsNullOrWhiteSpace(x.Attr.QueueName))
            .GroupBy(x => x.Type)
            .Select(g => (MessageType: g.Key, Queue: g.First().Attr!.QueueName))
            .ToList();
        
        services.AddRebus(configure =>
            configure
                .Transport(t => t.UseRabbitMq(rabbitMqConnString, inputQueueName))
                .Events(e => e.BeforeMessageSent +=(async (bus, headers, message, context) =>
                    {
                        var type = message.GetType();
                        var attr = type?.GetCustomAttribute<CommandAttribute>();
                        if (!string.IsNullOrWhiteSpace(attr?.RemoteTypeFullName))
                        {
                            headers["remote-type"] = attr.RemoteTypeFullName!;
                            headers[Headers.Type] = attr.RemoteTypeFullName!;
                        }
                    }))
                .Options(o =>
                {
                    o.RetryStrategy(
                        errorQueueName: $"{inputQueueName}-error",
                        maxDeliveryAttempts: 3);
                })
                .Serialization(s => s.Register(_ => 
                    new HeaderMappedJsonSerializer(
                        headerName: "remote-type", 
                        map: BuildRemoteNameToLocalTypeMap())))
                .Routing(r =>
                {
                    var tb = r.TypeBased();
                    foreach (var (messageType, queue) in messageRoutes)
                    {
                        tb.Map(messageType, queue);
                    }
                })
        );
        
        return services;
    }
    
    private static Assembly[] LoadAllAppAssemblies()
    {
        var set = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic && !IsFrameworkAssembly(a.GetName().Name))
            .GroupBy(a => a.GetName().Name, StringComparer.Ordinal)
            .Select(g => g.First())
            .ToDictionary(a => a.GetName().Name, a => a, StringComparer.Ordinal);

        var dc = DependencyContext.Default;
        if (dc != null)
        {
            foreach (var lib in dc.RuntimeLibraries)
            {
                if (IsFrameworkAssembly(lib.Name)) continue;

                foreach (var name in lib.GetDefaultAssemblyNames(dc))
                {
                    if (IsFrameworkAssembly(name.Name)) continue;

                    if (!set.ContainsKey(name.Name))
                    {
                        try
                        {
                            var asm = Assembly.Load(name);
                            if (!asm.IsDynamic)
                                set[asm.GetName().Name] = asm;
                        }
                        catch { /* ignore */ }
                    }
                }
            }
        }

        var result = set.Values
            .Where(a =>
            {
                var n = a.GetName().Name;
                return !n.EndsWith(".Tests", StringComparison.Ordinal) &&
                       !n.StartsWith("testhost", StringComparison.OrdinalIgnoreCase) &&
                       !n.Equals("DynamicProxyGenAssembly2", StringComparison.Ordinal);
            })
            .ToArray();

        return result;
    }
    private static bool IsFrameworkAssembly(string? name)
    {
        if (string.IsNullOrEmpty(name)) return true;
        return name.StartsWith("System.", StringComparison.Ordinal)
               || name.Equals("System", StringComparison.Ordinal)
               || name.StartsWith("Microsoft.", StringComparison.Ordinal)
               || name.Equals("Microsoft", StringComparison.Ordinal)
               || name.Equals("netstandard", StringComparison.Ordinal)
               || name.Equals("mscorlib", StringComparison.Ordinal);
    }

    private static Type[] SafeGetTypes(Assembly a)
    {
        try { return a.GetTypes(); }
        catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null)!.ToArray()!; }
    }
    
    private static IDictionary<string, Type> BuildRemoteNameToLocalTypeMap()
    {
        var assemblies = LoadAllAppAssemblies();
        return assemblies
            .SelectMany(SafeGetTypes)
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .Select(t => new { t, Attr = t.GetCustomAttribute<MessageAttribute>() })
            .Where(x => x.Attr is { TypeFullName: not null })
            .ToDictionary(x => x.Attr!.TypeFullName!, x => x.t, StringComparer.Ordinal);
    }
}