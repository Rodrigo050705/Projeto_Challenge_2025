using Henry.AI.Agent.Domain.Templates.Providers.Interfaces;
using Henry.AI.Agent.Domain.Templates.Tokens;
using Mgb.DependencyInjections.DependencyInjectons.Interfaces;

namespace Henry.AI.Agent.Domain.Templates.Providers;

public class RefactoringTemplateProvider : ITemplateProvider, ITransientDependency
{
    public TemplateType Type => TemplateType.RefactoringTemplate;

    public string ProvideTemplate()
    {
        return "Repita a mensagem a seguir e adicione Até logo! no final da mesma: //[Code]//";
    }
}