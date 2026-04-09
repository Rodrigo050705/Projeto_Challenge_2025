using Henry.AI.Core.Host.CodeUnderstanding.Contracts;
using Mgb.DependencyInjections.DependencyInjectons.Interfaces;
using Rebus.Bus;

namespace Henry.AI.Core.Host.CodeUnderstanding.Services;

public class CodeUnderstandingService : ICodeUnderstandingService,ITransientDependency 
{
    private readonly IBus _bus;

    public CodeUnderstandingService(IBus bus)
    {
        _bus = bus;
    }

    public async Task<bool> PublishRequest(string rawCode)
    {
        var message = new CodeUnderstandingRequest()
        {
            Code = rawCode,
        };

        
        await _bus.Send(message);
        return true;
    }
}