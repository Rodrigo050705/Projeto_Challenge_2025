using Henry.AI.Agent.Host.CodeUnderstanding.Dtos;

namespace Henry.AI.Agent.Host.CodeUnderstanding.Services;

public interface ICodeUnderstandingService
{
    Task<CodeUnderstandingRawCodeOutputDto> UnderstandRawCode(string code);
}