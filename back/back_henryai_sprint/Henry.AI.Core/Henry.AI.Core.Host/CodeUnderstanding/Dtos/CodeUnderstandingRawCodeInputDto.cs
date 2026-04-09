namespace Henry.AI.Core.Host.CodeUnderstanding.Dtos;

public record CodeUnderstandingRawCodeInputDto(string Code)
{
    public string Code { get; set; } = Code;
}