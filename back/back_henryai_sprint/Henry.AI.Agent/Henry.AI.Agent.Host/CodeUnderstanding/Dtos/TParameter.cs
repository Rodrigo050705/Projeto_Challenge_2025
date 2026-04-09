using System.Text.Json.Serialization;

namespace Henry.AI.Agent.Host.CodeUnderstanding.Dtos;


public class TParameter
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("Default value")]
    public string? DefaultValue { get; set; }

    [JsonPropertyName("Optional")]
    public bool Optional { get; set; }

    [JsonPropertyName("Annotations")]
    public List<TAnnotation> Annotations { get; set; } = new();
}