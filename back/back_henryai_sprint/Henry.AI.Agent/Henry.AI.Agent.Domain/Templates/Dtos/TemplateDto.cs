using Henry.AI.Agent.Domain.Templates.Tokens;

namespace Henry.AI.Agent.Domain.Templates.Dtos;

public class TemplateDto
{
    public string Template { get; set; }
    public string ReplacedTemplate { get; set; }
    public TemplateType TemplateType { get; set; }
    public bool Ok => ReplacedTemplate.Length > 0;

    public TemplateDto(TemplateType templateType,string template, string replacedTemplate)
    {
        Template = template;
        ReplacedTemplate = replacedTemplate;
        TemplateType = templateType;
    }
}