using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Henry.AI.Core.Host.CodeUnderstanding.Controllers;

[ApiController]
[Route("codeunderstanding")]
public class CodeUnderstandingProxyController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    public CodeUnderstandingProxyController(IHttpClientFactory http) => _http = http;

    public record RawCodeInput(string Code);

    [HttpPost("rawcode")]
    public async Task<IActionResult> RawCode([FromBody] RawCodeInput input)
    {
        var client = _http.CreateClient("agent");
        var res = await client.PostAsJsonAsync("/codeunderstanding/rawcode", input);
        var json = await res.Content.ReadAsStringAsync();
        return new ContentResult { StatusCode = (int)res.StatusCode, Content = json, ContentType = "application/json" };
    }
}