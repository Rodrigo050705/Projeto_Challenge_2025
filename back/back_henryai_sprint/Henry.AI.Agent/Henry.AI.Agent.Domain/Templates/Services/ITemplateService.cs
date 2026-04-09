using Henry.AI.Agent.Domain.Templates.Dtos;
using Henry.AI.Agent.Domain.Templates.Tokens;

namespace Henry.AI.Agent.Domain.Templates.Services;

public interface ITemplateService
{
    public TemplateDto BuildTemplate(TemplateType templateType, IReadOnlyDictionary<ReplaceProperty, string> replaceProperties);
}