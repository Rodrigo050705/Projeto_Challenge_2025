using System.Text.Json;
using Henry.AI.Core.Infrastructure.Neo4J.Interfaces;

namespace Henry.AI.Core.Domain.CodeUnderstanding.UnmappedEntities;
[Serializable]
public class Parameter
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Optional { get; set; } = false;
    public string DefaultValue { get; set; } = string.Empty;
    public List<Annotation> Annotations { get; set; } = new List<Annotation>();

    public Dictionary<string, object?> ToCypherParameters()
    {
        return new Dictionary<string, object?>
        {
            ["Name"] = Name,
            ["Type"] = Type,
            ["Description"] = Description,
            ["Optional"] = Optional,
            ["DefaultValue"] = DefaultValue,
            ["Annotations"] = JsonSerializer.Serialize(
                Annotations.Select(a => a.ToCypherParameters())
            ),
        };
    }
}