using Henry.AI.Agent.Host.CodeUnderstanding.Dtos;
using Henry.AI.Agent.Host.CodeUnderstanding.Services;
using Microsoft.AspNetCore.Mvc;

namespace Henry.AI.Agent.Host.CodeUnderstanding.Controllers;

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
    public async Task<IActionResult> CreateDocumentationFromRawCode([FromBody] CodeUnderstandingRawCodeInputDto inputDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _codeUnderstandingService.UnderstandRawCode(inputDto.Code);

        if (!response.OK)
        {
            return UnprocessableEntity(String.Join("\n", response.Errors.Select(x => x.Description)));
        }

        return Ok(response);

    }
    
}