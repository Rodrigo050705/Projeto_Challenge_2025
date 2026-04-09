namespace Henry.AI.Core.Host.CodeUnderstanding.Services;
public interface ICodeUnderstandingService
{
    Task<bool> PublishRequest(string rawCode);
}