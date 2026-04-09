using System.Collections.Concurrent;
using System.Text;
using Newtonsoft.Json;
using Rebus.Extensions;
using Rebus.Messages;
using Rebus.Serialization;

namespace Mgb.ServiceBus.ServiceBus;

public sealed class HeaderMappedJsonSerializer : ISerializer
{
    private readonly JsonSerializerSettings _settings = new() { TypeNameHandling = TypeNameHandling.None };
    private readonly string _headerName;
    private readonly ConcurrentDictionary<string, Type> _map;

    public HeaderMappedJsonSerializer(string headerName, IDictionary<string, Type> map)
    {
        _headerName = headerName;
        _map = new(map);
    }

    public Task<TransportMessage> Serialize(Message message)
    {
        var json = JsonConvert.SerializeObject(message.Body, _settings);
        var bytes = Encoding.UTF8.GetBytes(json);

        var headers = message.Headers.Clone();
        headers[Headers.ContentType] = "application/json;charset=utf-8";

        if (!headers.ContainsKey(Headers.Type))
            headers[Headers.Type] = message.Body.GetType().GetSimpleAssemblyQualifiedName();

        return Task.FromResult(new TransportMessage(headers, bytes));
    }

    public Task<Message> Deserialize(TransportMessage transportMessage)
    {
        var encoding = Encoding.UTF8;
        var json = encoding.GetString(transportMessage.Body);

        Type? targetType = null;

        if (transportMessage.Headers.TryGetValue(_headerName, out var remoteTypeName))
            _map.TryGetValue(remoteTypeName, out targetType);

        if (targetType == null && transportMessage.Headers.TryGetValue(Headers.Type, out var typeName))
            targetType = Type.GetType(typeName);

        var body = targetType == null
            ? JsonConvert.DeserializeObject(json, _settings)! 
            : JsonConvert.DeserializeObject(json, targetType, _settings)!;

        return Task.FromResult(new Message(transportMessage.Headers.Clone(), body));
    }
}