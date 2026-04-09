namespace Henry.AI.Core.Domain.CodeUnderstanding.Helpers;

public static class TypeKindHelper
{
    public static TypeKind GetTypeKindFromString(string kindString) =>
        (kindString ?? throw new ArgumentNullException(nameof(kindString)))
            .Trim()
            .ToUpperInvariant() switch
            {
                "CLASS"      => TypeKind.Class,
                "INTERFACE"  => TypeKind.Interface,
                "STRUCT"     => TypeKind.Struct,
                "ENUM"       => TypeKind.Enum,
                "ENUMERATE"  => TypeKind.Enum, 
                "DELEGATE"   => TypeKind.Delegate,
                "RECORD"     => TypeKind.Record,
                _ => TypeKind.NotFound
            };

    public static string ToStringValue(this TypeKind kind) =>
        kind switch
        {
            TypeKind.Class     => "CLASS",
            TypeKind.Interface => "INTERFACE",
            TypeKind.Struct    => "STRUCT",
            TypeKind.Enum      => "ENUM",
            TypeKind.Delegate  => "DELEGATE",
            TypeKind.Record    => "RECORD",
            TypeKind.NotFound    => "NOT_FOUND",
            _ => throw new ArgumentOutOfRangeException(
                nameof(kind), $"Unsupported kind: {kind}")
        };
}