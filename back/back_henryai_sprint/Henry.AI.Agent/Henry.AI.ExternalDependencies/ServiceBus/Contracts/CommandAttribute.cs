namespace Mgb.ServiceBus.ServiceBus.Contracts;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class CommandAttribute : Attribute
{
    public string QueueName { get; }
    public string? RemoteTypeFullName { get; } 
    public CommandAttribute(string queueName, string remoteTypeFullName)
    {
        QueueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        RemoteTypeFullName = remoteTypeFullName;
    }
}