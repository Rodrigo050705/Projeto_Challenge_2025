using System.Text.Json.Serialization;
using Henry.AI.Core.Domain.CodeUnderstanding.UnmappedEntities;

namespace Henry.AI.Core.Host.CodeUnderstanding.Dtos;


public class TParameter
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("Default value")]
    public string DefaultValue { get; set; } = string.Empty;

    [JsonPropertyName("Optional")]
    public bool Optional { get; set; }

    [JsonPropertyName("Annotations")]
    public List<TAnnotation> Annotations { get; set; } = new();


    public Parameter ToParameter()
    {
        var parameter = new Parameter();
        parameter.Name = Name;
        parameter.Type = Type;
        parameter.Description = Description;
        parameter.DefaultValue = DefaultValue;
        parameter.Optional = Optional;
        parameter.Annotations = Annotations.Select(a => a.ToAnnotation()).ToList();
        return parameter;
    }
}