using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzer.Application.DTOs.Errors;
using ResumeAnalyzer.Application.DTOs.Requests;
using ResumeAnalyzer.Application.Notes.Analysis.Commands;
using ResumeAnalyzer.Application.Notes.Analysis.Queries;
using ResumeAnalyzer.Application.Services;
using ResumeAnalyzer.Domain.Entities.Results;
using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Infrastructure.Data;
using ResumeAnalyzer.WebApi.Models;
using System.Text.Json;

namespace ResumeAnalyzer.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisController(IMediator mediator) : BaseHhController
{

    [HttpGet("vacancies")]
    public async Task<IActionResult> GetVacancies(CancellationToken ct)
    {
        // Вся магия за одной строчкой
        var vacancies = await mediator.Send(new GetVacanciesQuery(HhToken), ct);
        return Ok(vacancies);
    }

    [HttpPost("bulk-change-state")]
    public async Task<IActionResult> BulkMove([FromBody] BulkMoveRequest request, CancellationToken ct)
    {
        // Мы просто упаковываем данные и отправляем их "мозгу" (MediatR)
        var command = new BulkMoveCommand(
            HhToken,
            request.VacancyId,
            request.CollectionId,
            request.NegotiationIds,
            request.ActionId
        );

        var result = await mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze([FromBody] ScoringRequest request)
    {
        var result = await mediator.Send(new AnalyzeCommand(HhToken, request));

        if (result.IsCancelled)
            return StatusCode(499, new { isCancelled = true, message = result.ErrorMessage });

        if (result.ErrorMessage != null)
            return StatusCode(500, new { error = result.ErrorMessage });

        return Ok(result.Candidates);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetUserInfo(CancellationToken ct)
    {
        var userInfo = await mediator.Send(new GetUserInfoQuery(HhToken), ct);
        return Ok(userInfo);
    }

    [HttpGet("vacancies/{vacancyId}/collections")]
    public async Task<IActionResult> GetCollections(string vacancyId, CancellationToken ct)
    {
        try
        {
            var collections = await mediator.Send(new GetCollectionsQuery(HhToken, vacancyId), ct);
            return Ok(collections);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Ошибка при загрузке коллекций", details = ex.Message });
        }
    }

    [HttpPost("change-state")]
    public async Task<IActionResult> ChangeState([FromBody] ChangeStateRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new ChangeCandidateStateCommand(HhToken, request.NegotiationId, request.ActionId), ct);

        if (result.Success) return Ok();
        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> Cancel()
    {
        await mediator.Send(new CancelAnalysisCommand());
        return Ok(new { message = "Анализ прерван пользователем" });
    }

    [HttpGet("negotiations/{negotiationId}/actions")]
    public async Task<IActionResult> GetActions(string negotiationId, CancellationToken ct)
    {
        try
        {
            var actions = await mediator.Send(new GetAvailableActionsQuery(HhToken, negotiationId), ct);
            return Ok(actions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Не удалось загрузить действия HH", error = ex.Message });
        }
    }
}