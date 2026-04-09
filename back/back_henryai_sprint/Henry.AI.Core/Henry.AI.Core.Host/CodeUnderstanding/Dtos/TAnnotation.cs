using System.Text.Json.Serialization;
using Henry.AI.Core.Domain.CodeUnderstanding.UnmappedEntities;

namespace Henry.AI.Core.Host.CodeUnderstanding.Dtos;

public class TAnnotation
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Parameters")]
    public List<TParameter> Parameters { get; set; } = new();

    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;

    public Annotation ToAnnotation()
    {
        var annotation = new Annotation();
        annotation.Name = Name;
        annotation.Description = Description;
        annotation.Parameters = Parameters.Select(p => p.ToParameter()).ToList();
        return annotation;
    }
}