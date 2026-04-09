using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Henry.AI.Core.Host.Documentation.Models;

public class Documentation
{
    [BsonId] public ObjectId Id { get; set; }
    public string CodeHash { get; set; } = "";
    public string Title { get; set; } = "";
    public string Language { get; set; } = "";
    public string Function { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
