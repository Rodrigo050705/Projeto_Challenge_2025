using Henry.AI.Core.Domain.CodeUnderstanding;
using Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;
using Henry.AI.Core.Infrastructure.Neo4J;
using Henry.AI.Core.Infrastructure.Neo4J.Interfaces;
using Mgb.DependencyInjections.DependencyInjectons.Interfaces;
using Neo4jClient;

namespace Henry.AI.Core.Host.CodeUnderstanding.Repositories;

public class MethodRepository : BaseRepository, IMethodRepository, ITransientDependency
{
    public MethodRepository(IGraphClient graphClient) : base(graphClient)
    {
    }
    public async Task Delete(MethodNode node)
    {
        await GraphClient.Cypher
            .Match("(c:MethodNode)")
            .Where((MethodNode c) => c.Namespace == node.Namespace &&
                                     c.ClassName == node.ClassName &&
                                     c.Name  == node.Name)
            .DetachDelete("c")
            .ExecuteWithoutResultsAsync();
    }
}