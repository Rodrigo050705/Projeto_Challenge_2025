using Henry.AI.Agent.Host.CodeUnderstanding.Dtos;
using Mgb.ServiceBus.ServiceBus.Contracts;

namespace Henry.AI.Agent.Host.CodeUnderstanding.Contracts;

[Command("Henry.AI.Core","Henry.AI.Core.Host.CodeUnderstanding.Contracts.CodeUnderstandingResponse")]
public class CodeUnderstandingResponse : ICommand
{
    public string Namespace { get; set; } = string.Empty;
    public List<TClass> Classes { get; set; } = new();
    public List<TError> Errors { get; set; } = new();
    public bool Ok { get; set; }

    public CodeUnderstandingResponse() { }

    public CodeUnderstandingResponse(CodeUnderstandingRawCodeOutputDto codeUnderstandingRawCodeOutputDto)
    {
        Namespace = codeUnderstandingRawCodeOutputDto.Namespace;
        Classes = codeUnderstandingRawCodeOutputDto.Classes;
        Errors = codeUnderstandingRawCodeOutputDto.Errors;
        Ok = codeUnderstandingRawCodeOutputDto.OK;
    }
}