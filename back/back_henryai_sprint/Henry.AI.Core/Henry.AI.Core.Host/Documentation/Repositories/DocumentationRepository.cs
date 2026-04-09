using Doc = Henry.AI.Core.Host.Documentation.Models.Documentation;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Henry.AI.Core.Host.Documentation.Repositories;

public class DocumentationRepository : IDocumentationRepository
{
    private readonly IMongoCollection<Doc> _col;
    public DocumentationRepository(IMongoCollection<Doc> col) => _col = col;

    public async Task<Doc> Create(Doc d)
    {
        await _col.InsertOneAsync(d);
        return d;
    }

    public async Task<Doc?> Get(string id)
    {
        var oid = ObjectId.Parse(id);
        return await _col.Find(x => x.Id == oid).FirstOrDefaultAsync();
    }

    public async Task<(IEnumerable<Doc> items, long total)> Search(string? q, string? language, string? function, int skip, int limit)
    {
        var f = Builders<Doc>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(language))
            f &= Builders<Doc>.Filter.Eq(x => x.Language, language);

        if (!string.IsNullOrWhiteSpace(function))
            f &= Builders<Doc>.Filter.Regex(x => x.Function, new BsonRegularExpression(function, "i"));

        if (!string.IsNullOrWhiteSpace(q))
            f &= Builders<Doc>.Filter.Text(q);

        var find = _col.Find(f);
        var total = await find.CountDocumentsAsync();
        var items = await find.SortByDescending(x => x.CreatedAt)
                              .Skip(skip)
                              .Limit(Math.Clamp(limit, 1, 100))
                              .ToListAsync();
        return (items, total);
    }


public async Task<Doc?> Update(string id, string title, string language, string function, string content)
{
    if (!ObjectId.TryParse(id, out var oid)) return null;
    var filter = Builders<Doc>.Filter.Eq(x => x.Id, oid);
    var update = Builders<Doc>.Update
        .Set(x => x.Title, title ?? "")
        .Set(x => x.Language, language ?? "")
        .Set(x => x.Function, function ?? "")
        .Set(x => x.Content, content ?? "")
        .Set(x => x.UpdatedAt, DateTime.UtcNow);
    return await _col.FindOneAndUpdateAsync(filter, update,
        new FindOneAndUpdateOptions<Doc> { ReturnDocument = ReturnDocument.After });
}

public async Task<bool> Delete(string id)
{
    if (!ObjectId.TryParse(id, out var oid)) return false;
    var result = await _col.DeleteOneAsync(x => x.Id == oid);
    return result.DeletedCount > 0;
}

}
