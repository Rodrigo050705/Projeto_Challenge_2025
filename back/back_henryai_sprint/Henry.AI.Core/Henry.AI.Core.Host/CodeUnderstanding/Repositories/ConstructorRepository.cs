using Henry.AI.Core.Domain.CodeUnderstanding;
using Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;
using Henry.AI.Core.Infrastructure.Neo4J;
using Henry.AI.Core.Infrastructure.Neo4J.Interfaces;
using Mgb.DependencyInjections.DependencyInjectons.Interfaces;
using Neo4jClient;

namespace Henry.AI.Core.Host.CodeUnderstanding.Repositories;

public class ConstructorRepository : BaseRepository, IConstructorRepository, ITransientDependency
{
    public ConstructorRepository(IGraphClient graphClient) : base(graphClient)
    {
    }

    public async Task Delete(ConstructorNode node)
    {
        await GraphClient.Cypher
            .Match("(c:ConstructorNode)")
            .Where((ConstructorNode c) => c.ClassName == node.ClassName &&
                                          c.Namespace == node.Namespace)
            .DetachDelete("c")
            .ExecuteWithoutResultsAsync();
    }
}