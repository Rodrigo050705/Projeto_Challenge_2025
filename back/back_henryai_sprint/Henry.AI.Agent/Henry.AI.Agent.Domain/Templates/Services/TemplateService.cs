using System.Text.RegularExpressions;
using Henry.AI.Agent.Domain.Templates.Dtos;
using Henry.AI.Agent.Domain.Templates.Providers.Interfaces;
using Henry.AI.Agent.Domain.Templates.Tokens;
using Mgb.DependencyInjections.DependencyInjectons.Interfaces;

namespace Henry.AI.Agent.Domain.Templates.Services;

public class TemplateService :ITemplateService, ITransientDependency
{
    private readonly ITemplateProviderFactory _templateProviderFactory;
    private static readonly Regex Token = new(@"//\[(?<key>[^\[\]]+)\]//", RegexOptions.Compiled);

    public TemplateService(ITemplateProviderFactory templateProviderFactory)
    {
        _templateProviderFactory = templateProviderFactory;
    }

    public TemplateDto BuildTemplate(TemplateType templateType,IReadOnlyDictionary<ReplaceProperty,string> replaceProperties)
    {
        var provider = _templateProviderFactory.Get(templateType);
        var raw = provider.ProvideTemplate();
        var replacedTemplate = Replace(raw,replaceProperties);
        return new TemplateDto(templateType,raw,replacedTemplate);
    }
    
    private string Replace(string template, IReadOnlyDictionary<ReplaceProperty, string> map)
    {
        if (string.IsNullOrEmpty(template)) return template;
        if (map.Count == 0) return template;

        return Token.Replace(template, m =>
        {
            var key = m.Groups["key"].Value;

            if (Enum.TryParse<ReplaceProperty>(key, ignoreCase: true, out var rp) &&
                map.TryGetValue(rp, out var valueFromEnum))
            {
                return valueFromEnum ?? string.Empty;
            }
            
            return m.Value;
        });
    }
}