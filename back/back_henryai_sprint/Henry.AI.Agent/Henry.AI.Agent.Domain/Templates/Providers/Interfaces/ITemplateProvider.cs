using Henry.AI.Agent.Domain.Templates.Tokens;

namespace Henry.AI.Agent.Domain.Templates.Providers.Interfaces;

public interface ITemplateProvider
{
    TemplateType Type { get; }
    public string ProvideTemplate();
}