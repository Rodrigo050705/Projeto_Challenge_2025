using System.Text.Json;

namespace Henry.AI.Core.Domain.CodeUnderstanding.UnmappedEntities;
[Serializable]
public class Annotation
{
    public string Name { get; set; } = string.Empty;
    public List<Parameter> Parameters { get; set; } = new();
    public string Description { get; set; } = string.Empty;
    
    public Dictionary<string, object?> ToCypherParameters()
    {
        return new Dictionary<string, object?>
        {
            ["Name"] = Name,
            ["Description"] = Description,
            ["Parameters"] = JsonSerializer.Serialize(
                Parameters.Select(p => p.ToCypherParameters())
            ),
        };
    }
}