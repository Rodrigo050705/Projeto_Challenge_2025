using Henry.AI.Core.Host.CodeUnderstanding.Dtos;
using Henry.AI.Core.Host.CodeUnderstanding.Services;
using Microsoft.AspNetCore.Mvc;

namespace Henry.AI.Core.Host.CodeUnderstanding.Controllers;

[Route("codeunderstanding")]
public class CodeUnderstandingController : ControllerBase
{
    private readonly ICodeUnderstandingService _codeUnderstandingService;

    public CodeUnderstandingController(ICodeUnderstandingService codeUnderstandingService)
    {
        _codeUnderstandingService = codeUnderstandingService;
    }

    [HttpPost]
    [Route("rawcode")]
    public async Task<IActionResult> CreateDocumentationFromRawCode(
        [FromBody] CodeUnderstandingRawCodeInputDto inputDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _codeUnderstandingService.PublishRequest(inputDto.Code);

        if (response)
        {
            return Ok();
        }

        return UnprocessableEntity();
    }
}