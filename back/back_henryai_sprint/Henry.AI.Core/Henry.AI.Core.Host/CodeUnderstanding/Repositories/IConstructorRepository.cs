using Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;
using Henry.AI.Core.Infrastructure.Neo4J.Interfaces;

namespace Henry.AI.Core.Host.CodeUnderstanding.Repositories;

public interface IConstructorRepository
{
    Task Create<T>(T entity) where T : INeo4JEntity;
    Task<T?> Find<T>(T classToFind) where T : INeo4JEntity;
    
    Task Delete(ConstructorNode node);
}