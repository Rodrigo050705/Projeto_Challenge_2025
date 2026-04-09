namespace Henry.AI.Agent.Host.Refactoring.Dtos;

public record RefactoringRawCodeOutputDto(string Code)
{
    public string Code { get; set; } = Code;
}