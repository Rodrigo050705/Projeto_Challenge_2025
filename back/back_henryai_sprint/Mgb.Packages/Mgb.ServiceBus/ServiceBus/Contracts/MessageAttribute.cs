namespace Mgb.ServiceBus.ServiceBus.Contracts;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class MessageAttribute : Attribute
{
    public string? RemoteTypeFullName { get; } 
    public MessageAttribute(string remoteTypeFullName)
    {
        RemoteTypeFullName = remoteTypeFullName;
    }
}