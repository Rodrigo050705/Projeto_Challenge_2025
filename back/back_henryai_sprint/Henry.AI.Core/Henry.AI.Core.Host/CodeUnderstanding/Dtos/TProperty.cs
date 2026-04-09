using System.Text.Json.Serialization;
using Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;

namespace Henry.AI.Core.Host.CodeUnderstanding.Dtos;

public class TProperty
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("Accessibility")]
    public string Accessibility { get; set; } = string.Empty;

    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;

    public PropertyNode ToProperty(string className,string namespc)
    {
        var property = new PropertyNode();
        property.Namespace = namespc;
        property.Name = Name;
        property.Type = Type;
        property.Accessibility = Accessibility;
        property.Description = Description;
        property.ClassName = className;
        return property;
    }
}