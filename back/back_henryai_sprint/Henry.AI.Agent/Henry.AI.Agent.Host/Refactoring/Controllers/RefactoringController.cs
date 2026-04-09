using Henry.AI.Agent.Host.Documentation.Dtos;
using Henry.AI.Agent.Host.Refactoring.Dtos;
using Henry.AI.Agent.Host.Refactoring.Services;
using Microsoft.AspNetCore.Mvc;

namespace Henry.AI.Agent.Host.Refactoring.Controllers;

[Route("refactoring")]
public class RefactoringController : ControllerBase
{
    private readonly IRefactoringService _refactoringService;

    public RefactoringController(IRefactoringService refactoringService)
    {
        _refactoringService = refactoringService;
    }


    [HttpPost]
    [Route("rawcode")]
    public async Task<IActionResult> CreateRefactoring([FromBody] RefactoringRawCodeInputDto input)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _refactoringService.RefactorRawCode(input.Code);
        if (!string.IsNullOrEmpty(response))
        {
            return Ok(new RefactoringRawCodeOutputDto(response));
        }
        
        return UnprocessableEntity();
    }
    
}