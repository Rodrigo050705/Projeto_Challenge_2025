using System.Text.Json.Serialization;
using Henry.AI.Core.Domain.CodeUnderstanding;
using Henry.AI.Core.Domain.CodeUnderstanding.Helpers;
using Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;

namespace Henry.AI.Core.Host.CodeUnderstanding.Dtos;

public class TClass
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("Type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("Dependencies")]
    public List<TDependency> Dependencies { get; set; } = new();

    [JsonPropertyName("Implementations")]
    public List<TImplementation> Implementations { get; set; } = new();

    [JsonPropertyName("Constructors")]
    public List<TConstructor> Constructors { get; set; } = new();

    [JsonPropertyName("Heritages")]
    public List<THeritage> Heritages { get; set; } = new();

    [JsonPropertyName("Properties")]
    public List<TProperty> Properties { get; set; } = new();

    [JsonPropertyName("Methods")]
    public List<TMethod> Methods { get; set; } = new();


    public ClassNode ToClass(string namespc)
    {
        var classEntity = new ClassNode();
        classEntity.Name = Name;
        classEntity.Namespace = namespc;
        classEntity.KindString = Type.ToUpperInvariant();
        classEntity.Description = Description;
        return classEntity;
    }
}