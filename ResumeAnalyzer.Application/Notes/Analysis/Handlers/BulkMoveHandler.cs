using MediatR;
using Microsoft.Extensions.Logging;
using ResumeAnalyzer.Application.DTOs.Errors;
using ResumeAnalyzer.Application.DTOs.Results;
using ResumeAnalyzer.Application.Interfaces;
using ResumeAnalyzer.Application.Notes.Analysis.Commands;
using ResumeAnalyzer.Domain.Entities.Results;
using ResumeAnalyzer.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ResumeAnalyzer.Application.Notes.Analysis.Handlers
{
    public class BulkMoveHandler(
    IHeadHunterProvider hhProvider,
    IAppDbContext context, // Тот самый интерфейс, который мы создали!
    ILogger<BulkMoveHandler> logger)
    : IRequestHandler<BulkMoveCommand, BulkMoveResult>
    {
        public async Task<BulkMoveResult> Handle(BulkMoveCommand request, CancellationToken ct)
        {
            var results = new List<CandidateError>();
            int success = 0;

            foreach (var id in request.NegotiationIds)
            {
                var (isOk, error) = await hhProvider.ChangeCandidateStateAsync(request.Token, id, request.ActionId, ct);
                if (isOk) success++;
                else results.Add(new CandidateError(id, error));
            }

            // Сохраняем аудит через интерфейс контекста
            var audit = new AnalysisAudit
            {
                VacancyId = request.VacancyId,
                ActionId = request.ActionId,
                SuccessCount = success,
                FailCount = results.Count,
                Details = JsonSerializer.Serialize(results)
            };

            context.AnalysisAudits.Add(audit);
            await context.SaveChangesAsync(ct);

            return new BulkMoveResult(success, results.Count, results);
        }
    }
}
