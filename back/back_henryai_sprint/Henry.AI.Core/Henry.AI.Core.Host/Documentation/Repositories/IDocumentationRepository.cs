using Doc = Henry.AI.Core.Host.Documentation.Models.Documentation;

namespace Henry.AI.Core.Host.Documentation.Repositories;

public interface IDocumentationRepository
{
    Task<Doc> Create(Doc d);
    Task<Doc?> Get(string id);
    Task<(IEnumerable<Doc> items, long total)> Search(string? q, string? language, string? function, int skip, int limit);
    Task<Doc?> Update(string id, string title, string language, string function, string content);
    Task<bool> Delete(string id);
}
