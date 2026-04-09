using System.Text.Json.Serialization;

namespace Henry.AI.Agent.Host.CodeUnderstanding.Dtos;

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
    
}