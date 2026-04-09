using System.Text.Json.Serialization;

namespace Henry.AI.Agent.Host.CodeUnderstanding.Dtos;

public class TConstructor
{
    [JsonPropertyName("Parameters")]
    public List<TParameter> Parameters { get; set; } = new();

    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;
}