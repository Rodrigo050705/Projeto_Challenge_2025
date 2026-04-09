namespace Henry.AI.Agent.Domain.Templates.Tokens;

public static class BaseJsonTemplate
{
    public static string ProvideTemplate()
    {
        return @"
Seja conciso.
Suas descrições devem ser em português do brasil. Preserve o nome das classes e tudo definido em código.
Sua resposta deve ser em formato JSON.
Respeite as propriedades mencionadas e preencha apenas elas, nada mais.
Utilize camelCase para o nome das propriedades.
Respeite o tipo das propriedades descritas.
Caso você já tenha mencionado algo, não é necessário repetir o mesmo.

";
    }
}