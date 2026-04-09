namespace Henry.AI.Core.Infrastructure.Neo4J.Utils;

public static class CypherUtil
{
    public static string EscapeString(string value)
    {
        if (value == null) return "";

        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}