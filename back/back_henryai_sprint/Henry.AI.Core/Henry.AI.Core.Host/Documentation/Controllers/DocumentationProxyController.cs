using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Henry.AI.Core.Host.Documentation.Controllers;

[ApiController]
[Route("documentation")]
public class DocumentationProxyController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    public DocumentationProxyController(IHttpClientFactory http) => _http = http;

    public record RawCodeInput(string Code);

    [HttpPost("rawcode")]
    public async Task<IActionResult> RawCode([FromBody] RawCodeInput input)
    {
        var client = _http.CreateClient("agent");
        var res = await client.PostAsJsonAsync("/documentation/rawcode", input);
        var json = await res.Content.ReadAsStringAsync();
        return new ContentResult { StatusCode = (int)res.StatusCode, Content = json, ContentType = "application/json" };
    }
}