namespace Henry.AI.Core.Infrastructure.Neo4J.Interfaces;

public interface INeo4JEntity
{
    public string ToCypherCreate();
    public string ToCypherSearch();
    
    public Dictionary<string, object?> ToCypherParameters();
}