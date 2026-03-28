using ResumeAnalyzer.Domain.Entities.BusinessModels;
using ResumeAnalyzer.Domain.Entities.Results;
using ResumeAnalyzer.Domain.Interfaces.Models;

namespace ResumeAnalyzer.Domain.Interfaces;

public interface IHeadHunterProvider
{
    // Получить список активных вакансий
    Task<IEnumerable<Vacancy>> GetVacanciesAsync(string accessToken, CancellationToken ct = default);

    // Получить описание конкретной вакансии (для промпта)
    Task<Vacancy> GetVacancyDetailsAsync(string accessToken, string vacancyId, CancellationToken ct = default);

    // Загрузить всех кандидатов из коллекции откликов (с пагинацией внутри)
    Task<IEnumerable<Candidate>> GetCandidatesByCollectionAsync(
        string accessToken, 
        string vacancyId, 
        string collectionId, 
        CancellationToken ct = default);

    // Получить список доступных действий для кандидата (этапы перевода)
    Task<IEnumerable<HhAction>> GetAvailableActionsAsync(string accessToken, string negotiationId,
        CancellationToken ct = default);

    // Выполнить массовый перевод на этап
    Task<(bool Success, string ErrorMessage)> ChangeCandidateStateAsync(string accessToken, string negotiationId, string actionId, CancellationToken ct = default);
    Task<IEnumerable<NegotiationCollection>> GetCollectionsAsync(string accessToken, string vacancyId, CancellationToken ct = default);
    
    Task<string> ExchangeCodeForTokenAsync(string code, CancellationToken ct = default);

    Task<HhUserResponse> GetUserInfoAsync(string accessToken, CancellationToken ct = default);
}

