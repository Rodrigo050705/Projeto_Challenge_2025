using System.Text.Json.Serialization;

namespace Henry.AI.Agent.Host.CodeUnderstanding.Dtos;

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
}