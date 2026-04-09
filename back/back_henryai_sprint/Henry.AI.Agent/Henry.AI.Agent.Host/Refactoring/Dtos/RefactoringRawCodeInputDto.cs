namespace Henry.AI.Agent.Host.Refactoring.Dtos;

public record RefactoringRawCodeInputDto(string Code)
{
    public string Code { get; set; } = Code;
}