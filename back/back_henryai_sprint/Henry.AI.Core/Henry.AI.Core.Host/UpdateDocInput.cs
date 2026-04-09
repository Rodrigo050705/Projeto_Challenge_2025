// Separate file to avoid placing types after top-level statements.
public record UpdateDocInput(
    string? Title,
    string? Language,
    string? Function,
    string? Content,
    string? DocumentedCode
);
