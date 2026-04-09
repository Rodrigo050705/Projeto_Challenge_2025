using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;

namespace Henry.AI.Core.Host.CodeUnderstanding.Dtos;

public class TMethod
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Return")]
    public string Return { get; set; } = string.Empty;

    [JsonPropertyName("Parameters")]
    public List<TParameter> Parameters { get; set; } = new();

    [JsonPropertyName("Annotations")]
    public List<TAnnotation> Annotations { get; set; } = new();

    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;

    public MethodNode ToMethod(string className,string namespc)
    {
        var method = new MethodNode();
        method.Name = Name;
        method.Returns = Return;
        method.Namespace = namespc;
        method.Parameters = Parameters.Select(p => p.ToParameter()).ToList();
        method.Annotations = Annotations.Select(a => a.ToAnnotation()).ToList();
        method.Description = Description;
        method.ClassName = className;
        return method;
    }
}