namespace Henry.AI.Agent.Host.Documentation.Dtos;

public record DocumentationRawCodeInputDto(string Code)
{
    public string Code { get; set; } = Code;
}