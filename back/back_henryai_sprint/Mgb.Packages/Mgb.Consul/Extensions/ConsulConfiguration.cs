using System.IO;
using System.Threading.Tasks;
using Mgb.Consul.Dtos;
using Mgb.Consul.Tokens;
using Microsoft.Extensions.Configuration;

namespace Mgb.Consul.Extensions;

public static class ConsulConfiguration
{
    public static async Task<IConfigurationBuilder> AddConsulConfigurationAsync(
        this IConfigurationBuilder builder)
    {
        var client = ConsulProvider.Client;

        async Task LoadJsonConfigsFromPrefix(string prefix)
        {
            var kvPairs = await client.KV.List(prefix);

            if (kvPairs?.Response != null)
            {
                foreach (var kv in kvPairs.Response)
                {
                    if (kv.Value == null) continue;

                    var json = System.Text.Encoding.UTF8.GetString(kv.Value);

                    var jsonStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
                    builder.AddJsonStream(jsonStream);
                }
            }
        }
        var config = builder.Build();
        await LoadJsonConfigsFromPrefix(GeneralTokens.GeneralKeyPrefix);
        await LoadJsonConfigsFromPrefix(config["AppId"]);

        return builder;
    }
}