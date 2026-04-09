using Henry.AI.Core.Domain.CodeUnderstanding.Helpers;
using Henry.AI.Core.Infrastructure.Neo4J.Interfaces;
using Henry.AI.Core.Infrastructure.Neo4J.Utils;

namespace Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;

public class ClassNode : INeo4JEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Namespace { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public TypeKind Kind => TypeKindHelper.GetTypeKindFromString(KindString);

    public string KindString { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FullNamespace => Namespace + "." + Name;

    public ClassNode Update(ClassNode nodeToUpdateFrom)
    {
        this.Description = nodeToUpdateFrom.Description;
        this.Name = nodeToUpdateFrom.Name;
        this.KindString = nodeToUpdateFrom.KindString;
        this.Namespace = nodeToUpdateFrom.Namespace;
        return this;
    }
    
    
    public Dictionary<string, object?> ToCypherParameters()
    {
        return new Dictionary<string, object?>
        {
            ["FullNamespace"] = FullNamespace,
            ["Id"] = Id,
            ["Namespace"] = Namespace,
            ["Name"] = Name,
            ["KindString"] = KindString,
            ["Description"] = Description
        };
    }

    public string ToCypherCreate()
    {
        return "(entity:ClassNode {Id:$Id, Namespace:$Namespace, Name:$Name, KindString:$KindString, Description:$Description,FullNamespace:$FullNamespace})";
    }

    public string ToCypherSearch()
    {
        return "(entity:ClassNode {FullNamespace:$FullNamespace, KindString:$KindString})";
    }
}
