using Henry.AI.Core.Host.CodeUnderstanding.Dtos;
using Mgb.ServiceBus.ServiceBus.Contracts;

namespace Henry.AI.Core.Host.CodeUnderstanding.Contracts;

[Message("Henry.AI.Core.Host.CodeUnderstanding.Contracts.CodeUnderstandingResponse")]
public class CodeUnderstandingResponse : IMessage
{
    public string Namespace { get; set; } = string.Empty;
    public List<TClass> Classes { get; set; } = new();
    public List<TError> Errors { get; set; } = new();
    public bool Ok { get; set; }
}