using MediatR;
using ResumeAnalyzer.Application.DTOs.Requests;
using ResumeAnalyzer.Application.Notes.Analysis.Commands;
using ResumeAnalyzer.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Handlers
{
    public class AnalyzeHandler(
    CandidateScoringService scoringService,
    AnalysisManager analysisManager
) : IRequestHandler<AnalyzeCommand, AnalyzeResult>
    {
        public async Task<AnalyzeResult> Handle(AnalyzeCommand request, CancellationToken _)
        {
            // 1. Управляем токеном отмены через наш менеджер
            var ct = analysisManager.StartNew();

            try
            {
                // 2. Маппинг данных (переводим из Web-модели в Application-DTO)
                var appRequest = new AnalysisRequestDto(
                    request.Request.VacancyId,
                    request.Request.CollectionId,
                    request.Request.TopN,
                    request.Request.CustomCriteria,
                    request.Request.PlusRules,
                    request.Request.PenaltyRules
                );

                // 3. Вызываем сервис скоринга
                var topCandidates = await scoringService.GetTopCandidatesAsync(request.Token, appRequest, ct);

                return new AnalyzeResult(Candidates: topCandidates);
            }
            catch (OperationCanceledException)
            {
                return new AnalyzeResult(IsCancelled: true, ErrorMessage: "Анализ остановлен пользователем");
            }
            catch (Exception ex)
            {
                return new AnalyzeResult(ErrorMessage: ex.Message);
            }
        }
    }
}
