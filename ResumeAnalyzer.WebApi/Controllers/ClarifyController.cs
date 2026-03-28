using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ResumeAnalyzer.Application.Notes.Clarify.Commands;
using ResumeAnalyzer.Application.DTOs.Clarify;
using MediatR;

namespace ResumeAnalyzer.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ClarifyController(IMediator mediator) : BaseHhController
{
    [HttpPost("vacancies/{vacancyId}/start")]
    public async Task<IActionResult> StartClarify(string vacancyId)
    {
        var result = await mediator.Send(new StartClarifyCommand(vacancyId));
        return Ok(result);
    }

    [HttpPost("session/{sessionId}/answer")]
    public async Task<IActionResult> SubmitAnswer(string sessionId, [FromBody] UserAnswer answer)
    {
        try
        {
            var result = await mediator.Send(new SubmitAnswerCommand(sessionId, answer));
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}