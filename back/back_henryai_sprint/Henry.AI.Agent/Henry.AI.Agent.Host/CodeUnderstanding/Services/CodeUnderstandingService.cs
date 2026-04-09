using System.Text.Json;
using Henry.AI.Agent.Domain.Templates.Services;
using Henry.AI.Agent.Domain.Templates.Tokens;
using Henry.AI.Agent.Host.CodeUnderstanding.Dtos;
using Henry.AI.Agent.Host.OpenAI.Services;
using Mgb.DependencyInjections.DependencyInjectons.Interfaces;

namespace Henry.AI.Agent.Host.CodeUnderstanding.Services;

public class CodeUnderstandingService : ICodeUnderstandingService,ITransientDependency
{
    private TemplateType TemplateType => TemplateType.CodeUnderstandingTemplate;
    private readonly ITemplateService _templateService;
    private readonly IChatService _chatService;

    public CodeUnderstandingService(ITemplateService templateService, IChatService chatService)
    {
        _templateService = templateService;
        _chatService = chatService;
    }
    
    public async Task<CodeUnderstandingRawCodeOutputDto> UnderstandRawCode(string code)
    {
        var replaceProperties = new Dictionary<ReplaceProperty, string>();
        replaceProperties.Add(ReplaceProperty.Code,code);
        var templateDto = _templateService.BuildTemplate(TemplateType,replaceProperties);
        if (!templateDto.Ok)
        {
            return CreateErrorResponse("Template não foi montado corretamente.");
        }
        
        var response = await _chatService.SendMessageAsync(templateDto.ReplacedTemplate);
        if (!IsValidJson(response))
        {
            return CreateErrorResponse("JSON retornado de IA não está formatado corretamente");
        }

        try
        {
            var dto = JsonSerializer.Deserialize<CodeUnderstandingRawCodeOutputDto>(response,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return dto;
        }
        catch (Exception e)
        {
            return CreateErrorResponse(e.Message);
        }


    }


    private CodeUnderstandingRawCodeOutputDto CreateErrorResponse(string description)
    {
        var errorOutput = new CodeUnderstandingRawCodeOutputDto();
        errorOutput.OK = false;
        errorOutput.Errors.Add(new TError()
        {
            Description = description
        });
        return errorOutput;
    }
    private bool IsValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;
        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}