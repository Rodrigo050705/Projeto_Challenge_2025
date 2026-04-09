using Henry.AI.Agent.Host.CodeUnderstanding.Contracts;
using Henry.AI.Agent.Host.CodeUnderstanding.Services;
using Rebus.Bus;
using Rebus.Handlers;

namespace Henry.AI.Agent.Host.CodeUnderstanding.Handlers;

public class CodeUnderstandingRequestHandler : IHandleMessages<CodeUnderstandingRequest>
{
    private readonly ICodeUnderstandingService _codeUnderstandingService;
    private readonly IBus _bus;

    public CodeUnderstandingRequestHandler(ICodeUnderstandingService codeUnderstandingService, IBus bus)
    {
        _codeUnderstandingService = codeUnderstandingService;
        _bus = bus;
    }

    public async Task Handle(CodeUnderstandingRequest message)
    {
        var responseDto = await _codeUnderstandingService.UnderstandRawCode(message.Code);
        var responseMessage = new CodeUnderstandingResponse(responseDto);
        await _bus.Send(responseMessage);
    }

}