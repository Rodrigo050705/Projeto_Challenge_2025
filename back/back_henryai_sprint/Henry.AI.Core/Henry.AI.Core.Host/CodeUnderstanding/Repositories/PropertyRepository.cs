using Henry.AI.Core.Domain.CodeUnderstanding;
using Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;
using Henry.AI.Core.Infrastructure.Neo4J;
using Henry.AI.Core.Infrastructure.Neo4J.Interfaces;
using Mgb.DependencyInjections.DependencyInjectons.Interfaces;
using Neo4jClient;

namespace Henry.AI.Core.Host.CodeUnderstanding.Repositories;

public class PropertyRepository : BaseRepository, IPropertyRepository, ITransientDependency
{
    public PropertyRepository(IGraphClient graphClient) : base(graphClient)
    {
    }
    public async Task Delete(PropertyNode node)
    {
        await GraphClient.Cypher
            .Match("(c:PropertyNode)")
            .Where((PropertyNode c) => c.Namespace == node.Namespace &&
                                     c.ClassName == node.ClassName &&
                                     c.Name  == node.Name)
            .DetachDelete("c")
            .ExecuteWithoutResultsAsync();
    }
}