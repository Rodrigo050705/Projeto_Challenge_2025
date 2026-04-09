using Henry.AI.Agent.Host.Documentation.Dtos;
using Henry.AI.Agent.Host.Documentation.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Henry.AI.Agent.Host.Documentation.Controllers
{
    [Route("/documentation")]
    [ApiController]
    public class DocumentationController : ControllerBase
    {
        private readonly IDocumentationService _documentationService;

        public DocumentationController(IDocumentationService documentationService)
        {
            _documentationService = documentationService;
        }

        // JSON: { "code": "..." }
        [HttpPost]
        [Route("rawcode")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateDocumentationFromRawCode([FromBody] DocumentationRawCodeInputDto input)
        {
            if (input == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _documentationService.DocumentRawCode(input.Code);
            if (string.IsNullOrEmpty(response))
            {
                return UnprocessableEntity();
            }

            return Ok(new DocumentationRawCodeOutputDto(response));
        }

        // text/plain: raw code in body
        [HttpPost]
        [Route("rawcode")]
        [Consumes("text/plain")]
        public async Task<IActionResult> CreateDocumentationFromRawCodePlain([FromBody] string code)
        {
            code ??= string.Empty;
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("empty text/plain body");
            }

            var response = await _documentationService.DocumentRawCode(code);
            if (string.IsNullOrEmpty(response))
            {
                return UnprocessableEntity();
            }

            return Ok(new DocumentationRawCodeOutputDto(response));
        }
    }
}
