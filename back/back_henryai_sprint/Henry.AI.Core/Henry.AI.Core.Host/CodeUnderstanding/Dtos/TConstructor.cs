using System.Text.Json.Serialization;
using Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;

namespace Henry.AI.Core.Host.CodeUnderstanding.Dtos;

public class TConstructor
{
    [JsonPropertyName("Parameters")]
    public List<TParameter> Parameters { get; set; } = new();

    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;


    public ConstructorNode ToConstructor(string className,string namespc)
    {
        var constructor = new ConstructorNode();
        constructor.Description = Description;
        constructor.ClassName = className;
        constructor.Namespace = namespc;
        constructor.Parameters = Parameters.Select(p => p.ToParameter()).ToList();
        return constructor;
    }
}