using Henry.AI.Agent.Domain.Templates.Tokens;

namespace Henry.AI.Agent.Domain.Templates.Providers.Interfaces;

public interface ITemplateProviderFactory
{
    ITemplateProvider Get(TemplateType type);
    
}