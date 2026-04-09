using System.Text.Json;
using Henry.AI.Core.Domain.CodeUnderstanding.UnmappedEntities;
using Henry.AI.Core.Infrastructure.Neo4J.Interfaces;

namespace Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;

public class MethodNode : INeo4JEntity
{    
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Namespace { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Returns { get; set; } = string.Empty;
    public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    public List<Annotation> Annotations { get; set; } = new List<Annotation>();
    public string ToCypherCreate()
    {
        return "(entity:MethodNode {Id:$Id, Namespace:$Namespace, ClassName:$ClassName, Name:$Name, Description:$Description, Returns:$Returns, Parameters:$Parameters, Annotations:$Annotations})";
    }

    public string ToCypherSearch()
    {
        return "(entity:MethodNode {Namespace:$Namespace, ClassName:$ClassName, Name:$Name})";
    }

    public Dictionary<string, object?> ToCypherParameters()
    {
        return new Dictionary<string, object?>
        {
            ["Id"] = Id,
            ["Namespace"] = Namespace,
            ["ClassName"]   = ClassName,
            ["Name"]        = Name,
            ["Description"] = Description,
            ["Returns"]     = Returns,
            ["Parameters"]  = JsonSerializer.Serialize(Parameters.Select(p => p.ToCypherParameters())),
            ["Annotations"] = JsonSerializer.Serialize(Annotations.Select(a => a.ToCypherParameters())),
        };
    }
}