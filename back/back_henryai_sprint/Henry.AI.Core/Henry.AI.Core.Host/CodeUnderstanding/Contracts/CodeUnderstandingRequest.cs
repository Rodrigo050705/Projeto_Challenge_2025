using Mgb.ServiceBus.ServiceBus.Contracts;

namespace Henry.AI.Core.Host.CodeUnderstanding.Contracts;

[Command("Henry.AI.Agent","Henry.AI.Agent.Host.CodeUnderstanding.Contracts.CodeUnderstandingRequest")]
public class CodeUnderstandingRequest : ICommand
{
    public string Code { get; set; } = String.Empty;
}