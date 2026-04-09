using Henry.AI.Agent.Domain.Templates.Providers.Interfaces;
using Henry.AI.Agent.Domain.Templates.Tokens;
using Mgb.DependencyInjections.DependencyInjectons.Interfaces;

namespace Henry.AI.Agent.Domain.Templates.Providers;

public class DocumentationTemplateProvider : ITemplateProvider, ITransientDependency
{
    public TemplateType Type => TemplateType.DocumentationTemplate;

    public string ProvideTemplate()
    {
        return @"
Analise o código fornecido e gere documentação completa seguindo estas diretrizes:

1. **Descrição Geral**: Explique o propósito principal do código em linguagem simples
2. **Funcionalidades**: Liste as principais funcionalidades implementadas
3. **Parâmetros**: Documente todos os parâmetros de entrada (se aplicável)
4. **Retorno**: Descreva o que o código retorna (se aplicável)
5. **Regras de Negócio**: Identifique e explique as regras de negócio implementadas
6. **Exemplos de Uso**: Forneça exemplos práticos quando apropriado

**Requisitos:**
- Use comentários XML (///) para C# ou comentários apropriados para outras linguagens
- Evite jargões técnicos desnecessários
- Foque na lógica de negócio, não apenas na implementação técnica
- Use linguagem clara e acessível
: //[Code]//";
    }
}
