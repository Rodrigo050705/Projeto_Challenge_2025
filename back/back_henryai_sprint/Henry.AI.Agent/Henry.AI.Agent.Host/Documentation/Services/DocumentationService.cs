using Henry.AI.Agent.Domain.Templates.Services;
using Henry.AI.Agent.Domain.Templates.Tokens;
using Henry.AI.Agent.Host.OpenAI.Services;
using Mgb.DependencyInjections.DependencyInjectons.Interfaces;


namespace Henry.AI.Agent.Host.Documentation.Services;

public class DocumentationService : IDocumentationService,ITransientDependency
{
    private TemplateType TemplateType => TemplateType.DocumentationTemplate;
    private readonly ITemplateService _templateService;
    private readonly IChatService _chatService;

    public DocumentationService(ITemplateService templateService, IChatService chatService)
    {
        _templateService = templateService;
        _chatService = chatService;
    }
    
    public async Task<string> DocumentRawCode(string code)
    {
        var replaceProperties = new Dictionary<ReplaceProperty, string>();
        replaceProperties.Add(ReplaceProperty.Code,code);
        var templateDto = _templateService.BuildTemplate(TemplateType,replaceProperties);
        if (!templateDto.Ok)
        {
            return "";
        }

        var response = await _chatService.SendMessageAsync(templateDto.ReplacedTemplate);
        return response;
    }
}