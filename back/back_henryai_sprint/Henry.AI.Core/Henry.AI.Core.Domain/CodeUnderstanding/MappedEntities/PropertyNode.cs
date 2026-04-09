using System.Text.Json;
using Henry.AI.Core.Domain.CodeUnderstanding.UnmappedEntities;
using Henry.AI.Core.Infrastructure.Neo4J.Interfaces;

namespace Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;

public class PropertyNode: INeo4JEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Namespace { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Accessibility { get; set; } = string.Empty;
    public List<Annotation> Annotations { get; set; } = new List<Annotation>();
    public string ToCypherCreate()
    {
        return "(entity:PropertyNode {Id:$Id,Namespace:$Namespace,ClassName:$ClassName, Name:$Name, Type:$Type, Description:$Description, Accessibility:$Accessibility, Annotations:$Annotations})";
    }

    public string ToCypherSearch()
    {
        return "(entity:PropertyNode {Namespace:$Namespace, ClassName:$ClassName, Name:$Name})";
    }

    public Dictionary<string, object?> ToCypherParameters()
    {
        return new Dictionary<string, object?>
        {
            ["Id"] = Id,
            ["Namespace"] = Namespace,
            ["ClassName"] = ClassName,
            ["Name"] = Name,
            ["Type"] = Type,
            ["Description"] = Description,
            ["Accessibility"] = Accessibility,
            ["Annotations"] = JsonSerializer.Serialize(
                Annotations.Select(a => a.ToCypherParameters())
            ),
        };
    }
}