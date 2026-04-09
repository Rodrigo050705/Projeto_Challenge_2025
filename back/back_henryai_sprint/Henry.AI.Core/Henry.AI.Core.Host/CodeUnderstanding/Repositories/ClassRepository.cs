using Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;
using Henry.AI.Core.Domain.CodeUnderstanding.UnmappedEntities;
using Henry.AI.Core.Infrastructure.Neo4J;
using Mgb.DependencyInjections.DependencyInjectons.Interfaces;
using Neo4jClient;

namespace Henry.AI.Core.Host.CodeUnderstanding.Repositories;

public class ClassRepository : BaseRepository, IClassRepository, ITransientDependency
{
    public ClassRepository(IGraphClient graphClient) : base(graphClient)
    {
    }

    public async Task Update(ClassNode nodeToUpdate)
    {
        await GraphClient.Cypher
            .Match("(c:ClassNode)")
            .Where((ClassNode c) => c.Id == nodeToUpdate.Id)
            .Set("c = $updatedNode")
            .WithParam("updatedNode", nodeToUpdate)
            .ExecuteWithoutResultsAsync();
    }

    public async Task<ClassNode?> FindByNameOnly(ClassNode nodeToFind)
    {
        var node =  await GraphClient.Cypher
            .Match("(c:ClassNode)")
            .Where((ClassNode c) => c.Name == nodeToFind.Name)
            .WithParam("c", nodeToFind)
            .Return<ClassNode?>("c")
            .ResultsAsync;
        return node.FirstOrDefault();
    }

    public async Task CreateEmpty(ClassNode nodeToCreate)
    {
        await GraphClient.Cypher
            .Create(("(c:ClassNode {Name:$Name, Id:$Id})"))
            .WithParam("Name", nodeToCreate.Name)
            .WithParam("Id", nodeToCreate.Id)
            .ExecuteWithoutResultsAsync();
    }
    
    public async Task CreateInheritance(ClassNode child, ClassNode parent)
    {
        await GraphClient.Cypher
            .Match("(c:ClassNode)", "(p:ClassNode)")
            .Where((ClassNode c) => c.Id == child.Id)
            .AndWhere((ClassNode p) => p.Id == parent.Id)
            .Merge("(c)-[:INHERITS]->(p)")
            .ExecuteWithoutResultsAsync();
    }
    
    public async Task CreateDependency(ClassNode child, ClassNode parent, DependencyProperties dependencyProperties)
    {
        await GraphClient.Cypher
            .Match("(c:ClassNode)", "(p:ClassNode)")
            .Where((ClassNode c) => c.Id == child.Id)
            .AndWhere((ClassNode p) => p.Id == parent.Id)
            .Merge("(c)-[r:DEPENDS_ON]->(p)")
            .Set("r = $props")
            .WithParam("props", dependencyProperties)
            .ExecuteWithoutResultsAsync();
    }
    public async Task CreateImplementation(ClassNode child, ClassNode parent)
    {
        await GraphClient.Cypher
            .Match("(c:ClassNode)", "(p:ClassNode)")
            .Where((ClassNode c) => c.Id == child.Id)
            .AndWhere((ClassNode p) => p.Id == parent.Id)
            .Merge("(c)-[:IMPLEMENTS]->(p)")
            .ExecuteWithoutResultsAsync();
    }
    
    public async Task CreateConstructedBy(ClassNode child, ConstructorNode parent)
    {
        await GraphClient.Cypher
            .Match("(c:ClassNode)", "(p:ConstructorNode)")
            .Where((ClassNode c) => c.Id == child.Id)
            .AndWhere((ClassNode p) => p.Id == parent.Id)
            .Merge("(c)-[:CONSTRUCTED_BY]->(p)")
            .ExecuteWithoutResultsAsync();
    }
    
    public async Task CreateHasMethod(ClassNode child, MethodNode parent)
    {
        await GraphClient.Cypher
            .Match("(c:ClassNode)", "(p:MethodNode)")
            .Where((ClassNode c) => c.Id == child.Id)
            .AndWhere((ClassNode p) => p.Id == parent.Id)
            .Merge("(c)-[:HAS_METHOD]->(p)")
            .ExecuteWithoutResultsAsync();
    }
    
    public async Task CreateHasProperty(ClassNode child, PropertyNode parent)
    {
        await GraphClient.Cypher
            .Match("(c:ClassNode)", "(p:PropertyNode)")
            .Where((ClassNode c) => c.Id == child.Id)
            .AndWhere((ClassNode p) => p.Id == parent.Id)
            .Merge("(c)-[:HAS_PROPERTY]->(p)")
            .ExecuteWithoutResultsAsync();
    }
}