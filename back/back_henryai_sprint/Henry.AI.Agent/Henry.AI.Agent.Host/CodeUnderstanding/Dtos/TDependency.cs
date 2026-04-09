using System.Text.Json.Serialization;

namespace Henry.AI.Agent.Host.CodeUnderstanding.Dtos;

public class TDependency
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Accessibility")]
    public string Accessibility { get; set; } = string.Empty;
    
    [JsonPropertyName("Injected")]
    public bool Injected { get; set; }
}