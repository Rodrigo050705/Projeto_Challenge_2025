using Mgb.ServiceBus.ServiceBus.Contracts;

namespace Henry.AI.Agent.Host.CodeUnderstanding.Contracts;

[Message("Henry.AI.Agent.Host.CodeUnderstanding.Contracts.CodeUnderstandingRequest")]
public class CodeUnderstandingRequest : IMessage
{
    public string Code { get; set; } = String.Empty;
}