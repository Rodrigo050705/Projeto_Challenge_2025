using System.Text.Json.Serialization;

namespace Henry.AI.Agent.Host.CodeUnderstanding.Dtos;

public class THeritage
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;
}