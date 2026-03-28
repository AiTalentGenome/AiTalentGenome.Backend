using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Application.DTOs;
using ResumeAnalyzer.Application.DTOs.Requests;
using ResumeAnalyzer.Application.DTOs.Results;
using ResumeAnalyzer.Domain.Entities.BusinessModels;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Domain.Entities.AnalysisModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ResumeAnalyzer.Application.Hubs; // Для AnalysisResult

namespace ResumeAnalyzer.Application.Services;

public class CandidateScoringService(
    IHeadHunterProvider hhProvider, 
    IOllamaProvider ollamaProvider,
    IAnalysisCacheRepository cacheRepository,
    IHubContext<AnalysisProgressHub> hubContext,
    ILogger<CandidateScoringService> logger
    ) 
{
    public async Task<IEnumerable<CandidateResultDto>> GetTopCandidatesAsync(
        string token,
        AnalysisRequestDto request,
        CancellationToken ct)
    {
        logger.LogInformation("Запуск анализа вакансии {VacancyId}, этап {CollectionId}", request.VacancyId, request.CollectionId);
        
        var fullVacancy = await hhProvider.GetVacancyDetailsAsync(token, request.VacancyId, ct);
        
        var candidates = (await hhProvider.GetCandidatesByCollectionAsync(
            token,
            request.VacancyId,
            request.CollectionId,
            ct)).ToList();

        var results = new List<CandidateResultDto>();

        for (int i = 0; i < candidates.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            var candidate = candidates[i];
            logger.LogInformation("Обработка резюме: {ResumeId} (Кандидат: {FullName})", 
                            candidate.ResumeId, candidate.FullName);
            
            try
            {
                string criteriaHash = GenerateCriteriaHash(request.PlusRules, request.PenaltyRules);
                
                string cacheKey = $"v_{request.VacancyId}_r_{candidate.ResumeId}_c_{criteriaHash}";

                // 3. Пытаемся достать результат из кэша (SQLite)
                var cachedJson = await cacheRepository.GetCachedResultAsync(cacheKey, ct);
                AnalysisResult? analysis = null;

                if (!string.IsNullOrEmpty(cachedJson))
                {
                    // Если нашли в кэше — десериализуем
                    analysis = JsonSerializer.Deserialize<AnalysisResult>(cachedJson);
                    Console.WriteLine($"[CACHE] Hit for candidate: {candidate.FullName}");
                }
                else
                {
                    // 4. Если в кэше пусто — идем в Ollama
                    string prompt = BuildPrompt(fullVacancy, candidate, request);
                    analysis = await ollamaProvider.AnalyzeResumeAsync(prompt, ct);
                    
                    logger.LogInformation("LLM завершила скоринг для {ResumeId}. Балл: {Score}", 
                        candidate.ResumeId, analysis.Score);

                    if (analysis != null)
                    {
                        // Сохраняем свежий результат в кэш
                        var jsonToCache = JsonSerializer.Serialize(analysis);
                        await cacheRepository.SaveToCacheAsync(cacheKey, jsonToCache, ct);
                        Console.WriteLine($"[OLLAMA] Analyzed and cached: {candidate.FullName}");
                    }
                }

                if (analysis == null) continue;

                results.Add(new CandidateResultDto(
                    candidate.NegotiationId ?? "no-id",
                    candidate.FullName ?? "Unknown",
                    analysis.Score,
                    analysis.RationaleRu ?? "Обоснование отсутствует",
                    candidate.ResumeUrl ?? "",
                    analysis.Flags?.MissingCoverLetter ?? false
                ));

                int percent = (int)((i + 1) / (double)candidates.Count * 100);

                await hubContext.Clients.All.SendAsync("ReceiveProgress", percent, ct);

                Console.WriteLine($"[PROGRESS] {percent}% - Обработан {candidate.FullName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to process candidate {candidate.FullName}: {ex.Message}");
            }
        }

        // 5. Сортировка по баллу (ТЗ 3.3)
        return results
            .OrderByDescending(r => r.Score)
            .Take(request.TopN);
    }

    private string GenerateCriteriaHash(List<ScoringRuleDto> plus, List<ScoringRuleDto> penalty)
    {
        var rulesStr = JsonSerializer.Serialize(new
        {
            p = plus.OrderBy(r => r.Description),
            m = penalty.OrderBy(r => r.Description)
        });
        
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rulesStr));
        return Convert.ToHexString(hashBytes)[..16];
    }

    private string BuildPrompt(Vacancy v, Candidate c, AnalysisRequestDto request)
    {
        var plusSection = string.Join("\n", request.PlusRules.Select(r => $"- [+{r.Points} баллов] {r.Description}"));
        var penaltySection = string.Join("\n", request.PenaltyRules.Select(r => $"- [-{r.Points} баллов] {r.Description}"));
        
        return $@"
        Ты — строгий технический эксперт-рекрутер. Твоя задача: провести беспристрастный скоринг резюме.
        
        ### КОНТЕКСТ ВАКАНСИИ
        Название: {v.Name}
        Описание: {v.Description}

        ### ДАННЫЕ КАНДИДАТА
        Имя: {c.FullName}
        Позиция: {c.DesiredPosition}
        Опыт: {c.BriefExperience}
        Сопроводительное письмо: {c.CoverLetter}

        ### ИНСТРУКЦИЯ ПО СКОРИНГУ
        1. Начни с базового счета 0 баллов.
        2. Применяй ПЛЮСЫ только при явном наличии подтвержденного опыта:
        {plusSection}

        3. Вычитай ШТРАФЫ при обнаружении несоответствий:
        {penaltySection}

        4. Итоговый балл не может быть меньше 0 и больше 100.
        5. Если в правиле указано конкретное количество баллов, используй именно его.

        ### ФОРМАТ ОТВЕТА (JSON)
        В ответ пришли ТОЛЬКО валидный JSON-объект. Не добавляй никакого вступительного или пояснительного текста.
        {{
          ""score"": integer,
          ""rationale_ru"": ""Три предложения: 1. Главный аргумент за. 2. Основная причина штрафа (если есть). 3. Рекомендация к найму.""
        }}";
    }
}