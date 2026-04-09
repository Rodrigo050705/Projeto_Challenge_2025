namespace Henry.AI.Agent.Host.Refactoring.Services;

public interface IRefactoringService
{
    Task<string> RefactorRawCode(string code);
}