namespace Henry.AI.Agent.Host.Documentation.Services;

public interface IDocumentationService
{
    Task<string> DocumentRawCode(string code);
}