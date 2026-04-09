using System.Text.Json.Serialization;

namespace Henry.AI.Core.Host.CodeUnderstanding.Dtos;

public class TError
{
    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;
}