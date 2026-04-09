using Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;
using Henry.AI.Core.Domain.CodeUnderstanding.UnmappedEntities;
using Henry.AI.Core.Infrastructure.Neo4J.Interfaces;

namespace Henry.AI.Core.Host.CodeUnderstanding.Repositories;

public interface IClassRepository
{
    Task Create<T>(T entity) where T : INeo4JEntity;
    Task<T?> Find<T>(T classToFind) where T : INeo4JEntity;
    
    Task Update(ClassNode nodeToUpdate);

    Task<ClassNode?> FindByNameOnly(ClassNode nodeToFind);
    Task CreateEmpty(ClassNode nodeToCreate);
    Task CreateInheritance(ClassNode child, ClassNode parent);
    Task CreateImplementation(ClassNode child, ClassNode parent);
    Task CreateDependency(ClassNode child, ClassNode parent, DependencyProperties dependencyProperties);
    Task CreateConstructedBy(ClassNode child, ConstructorNode parent);
    Task CreateHasMethod(ClassNode child, MethodNode parent);
    Task CreateHasProperty(ClassNode child, PropertyNode parent);
}