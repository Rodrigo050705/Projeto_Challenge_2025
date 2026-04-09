using Henry.AI.Core.Infrastructure.Neo4J.Interfaces;
using Neo4jClient;

namespace Henry.AI.Core.Infrastructure.Neo4J;

public abstract class BaseRepository
{
    protected IGraphClient GraphClient;

    protected BaseRepository(IGraphClient graphClient)
    {
        GraphClient = graphClient;
    }

    public async Task Create<T>(T entityToCreate) where T : INeo4JEntity
    {
        await GraphClient.Cypher
            .Create(entityToCreate.ToCypherCreate())
            .WithParams(entityToCreate.ToCypherParameters())
            .ExecuteWithoutResultsAsync();
    }
    
    public async Task<T?> Find<T>(T entityToFind) where T : INeo4JEntity
    {
        var result = await GraphClient.Cypher
            .Match(entityToFind.ToCypherSearch())
            .WithParams(entityToFind.ToCypherParameters())
            .Return<T>("entity").ResultsAsync;
        return result.FirstOrDefault();
    }
}