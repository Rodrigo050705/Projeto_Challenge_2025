namespace Mgb.ServiceBus.ServiceBus.Contracts;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class MessageAttribute : Attribute
{
    public string? TypeFullName { get; } 
    public MessageAttribute(string typeFullName)
    {
        TypeFullName = typeFullName;
    }
}