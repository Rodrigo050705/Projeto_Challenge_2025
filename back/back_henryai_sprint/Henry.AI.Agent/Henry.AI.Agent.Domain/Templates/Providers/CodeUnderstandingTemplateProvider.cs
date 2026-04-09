using Henry.AI.Agent.Domain.Templates.Providers.Interfaces;
using Henry.AI.Agent.Domain.Templates.Tokens;
using Mgb.DependencyInjections.DependencyInjectons.Interfaces;

namespace Henry.AI.Agent.Domain.Templates.Providers;

public class CodeUnderstandingTemplateProvider : ITemplateProvider, ITransientDependency
{
    public TemplateType Type => TemplateType.CodeUnderstandingTemplate;
    public string ProvideTemplate()
    {
        string baseJsonResponseTemplate = BaseJsonTemplate.ProvideTemplate();
        var template = baseJsonResponseTemplate += @"
Analise o código a seguir com os seguintes critérios:
1) Trate-o como um arquivo de classe em C#.
2) Entenda as propriedades existentes na classe e seus métodos.
3) Entenda as dependências que essa classe possui, suas heranças e implementações.
4) Não altere nada do código, seu propósito é apenas o entendimento e documentação do código no formato requisitado.
5) Caso exista erro na análise por qualquer motivo, preencha apenas o array de Errors e altera a flag OK.

O JSON deve ter as seguintes propriedades:
Namespace: String do namespace onde a classe se encontra.
Classes: Um array de objetos do tipo TClass das classes do código analisado
Errors: Um array de objetos do tipo TError, preenchido apenas se houver um erro na análise.
OK: Uma flag booleana para sinalizar que a análise ocorreu como o esperado

Os tipos mencionados possuem as seguintes propriedades, em ordem:

TClass:
Name: Uma string que define o nome da classe.
Description: Sua descricão para esta classe.
Type: Uma string que define o tipo, se é classe, interface, enum ou etc.
Dependencies: Um array de objetos do tipo TDependency das dependencias existentes da classe analisada. Considere apenas como dependencia aquilo que é declarado em construtores por DI ou chamadas de métodos estáticos.
Implementations: Um array de objetos do tipo TImplementation de interfaces da classe analisada. Considere apenas aquilo que for declarado.
Constructors: Um array de objetos do tipo TConstructor representando os construtores da classe analisada.
Heritages: Um array de objetos do tipo THeritage das heranças da classe analisada. Considere apenas aquilo que for declarativo.
Properties: Um array de objetos do tipo TProperty composto pelas propriedades da classe analisada. Considere propriedades apenas aquilo que é explicitamente declarado como propriedade, ignore fields.
Methods: Um array de objetos do tipo TMethod representando os métodos da classe analisada


TDependency:
1)Name: Nome da classe,interface e afim.
2)Accessibility: Acessibilidade da dependencia, ex: public, private...
3)Injected: Booleano que determina se a dependência foi injetada.

TImplementation:
1)Name: Nome da interface implementada

THeritage:
1)Name: Nome da classe herdada

TProperty:
1)Name: Nome da propriedade
2)Type: Tipo da propriedade
3)Accessibility: Acessibilidade da dependencia, ex: public, private...
4)Description: Sua descricão para esta propriedade
5)Annotations: Array de objetos do tipo TAnnotation da propriedade analisada3333

TMethod:
1)Name: Nome do método
2)Return: Tipo de retorno do método
3)Parameters: Array de objetos do tipo TParameter dos parametros do método analisado
4)Annotations: Array de objetos do tipo TAnnotation do método analisado
5)Description: Sua descrição para este método

TParameter:
1)Name: Nome do parametro
2)Type: Tipo do parametro
3)Description: Sua descrição do parametro
4)Default value: Caso possua default value, seu valor
5)Optional: Boolean para sinalizar se o parametro é opcional ou não
6)Annotations: Array de objetos do tipo TAnnotation do parametro analisado

TAnnotation:
1)Name: Nome da annotation
2)Parameters: Array de objetos do tipo TParameter dos parâmetros da annotation.
3)Description: Sua descrição para esta annotation

TConstructor:
1)Parameters: Array de objetos do tipo TParameter dos parametros do método analisado
2)Description: Sua descrição para este construtor

TError:
1)Description: Uma curta descrição do motivo de a análise não poder ser concluída


Com isso dito, aqui está o código a ser analisado:

//[CODE]//

";
        return template;
    }
}