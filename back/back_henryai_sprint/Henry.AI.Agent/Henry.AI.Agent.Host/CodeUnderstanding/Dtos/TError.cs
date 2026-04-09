using System.Text.Json.Serialization;

namespace Henry.AI.Agent.Host.CodeUnderstanding.Dtos;

public class TError
{
    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;
}