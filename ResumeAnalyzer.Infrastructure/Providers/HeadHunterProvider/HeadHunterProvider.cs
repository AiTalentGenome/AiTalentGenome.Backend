using ResumeAnalyzer.Domain.Entities.BusinessModels;
using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ResumeAnalyzer.Domain.Configuration;
using ResumeAnalyzer.Domain.Entities.Results;
using ResumeAnalyzer.Domain.Interfaces.Models;

namespace ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider;

public class HeadHunterProvider : IHeadHunterProvider
{
    private readonly HttpClient _httpClient;
    private readonly HhOptions _options;
    private readonly ILogger<HeadHunterProvider> _logger;

    public HeadHunterProvider(HttpClient httpClient, IOptions<HhOptions> options, ILogger<HeadHunterProvider> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri("https://api.hh.ru/");
        
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ResumeAnalyzerApp/1.0 (thirty.sixth@yandex.ru)");
        _logger = logger;
    }

    public async Task<IEnumerable<Vacancy>> GetVacanciesAsync(string accessToken, CancellationToken ct = default)
    {
        SetAuthHeader(accessToken);

        // 1. Узнаем ID компании динамически
        var employerId = await GetCurrentEmployerIdAsync(accessToken, ct);

        if (string.IsNullOrEmpty(employerId))
        {
            // Если это личный аккаунт (соискатель), вакансий работодателя не будет
            return Enumerable.Empty<Vacancy>();
        }

        // 2. Запрашиваем вакансии именно этой компании
        var url = $"vacancies?employer_id={employerId}&per_page=100";
        var response = await _httpClient.GetFromJsonAsync<HhVacanciesResponse>(url, ct);

        return response?.Items ?? Enumerable.Empty<Vacancy>();
    }

    public async Task<Vacancy> GetVacancyDetailsAsync(string accessToken, string vacancyId,
        CancellationToken ct = default)
    {
        SetAuthHeader(accessToken);

        // Запрос к конкретной вакансии для получения полного текста
        var url = $"vacancies/{vacancyId}";

        // В HH API полное описание лежит в поле "description"
        var vacancy = await _httpClient.GetFromJsonAsync<HhVacancyDetailResponse>(url, ct);

        return new Vacancy
        {
            Id = vacancy.Id,
            Name = vacancy.Name,
            Description = vacancy.Description // Теперь здесь будет полный HTML/текст
        };
    }

    public async Task<IEnumerable<Candidate>> GetCandidatesByCollectionAsync(string token, string vacancyId,
        string collectionId, CancellationToken ct)
    {
        SetAuthHeader(token);
        var allCandidates = new List<Candidate>();
        int currentPage = 0;
        int totalPages = 0;

        using var semaphore = new SemaphoreSlim(5);

        do
        {
            var url = $"negotiations/{collectionId}?vacancy_id={vacancyId}&per_page=50&page={currentPage}";

            // --- НАЧАЛО БЛОКА RETRY (ЗАЩИТА ОТ 502 ОШИБКИ) ---
            HttpResponseMessage responseMessage = null;
            int maxRetries = 3; // Пробуем 3 раза

            for (int i = 0; i < maxRetries; i++)
            {
                responseMessage = await _httpClient.GetAsync(url, ct);

                // Если всё хорошо (200 OK) — выходим из цикла попыток
                if (responseMessage.IsSuccessStatusCode) break;

                // Если ошибка 500, 502, 503 или 504 — это временный сбой HH
                if ((int)responseMessage.StatusCode >= 500)
                {
                    Console.WriteLine(
                        $"[HH Warning] Сервер HH вернул {responseMessage.StatusCode}. Попытка {i + 1}...");
                    await Task.Delay(1000 * (i + 1), ct); // Ждем чуть-чуть перед следующей попыткой
                    continue;
                }

                // Если ошибка 400 или 404 — переповтор не поможет, выходим
                break;
            }
            // --- КОНЕЦ БЛОКА RETRY ---

            // Если после всех попыток всё еще ошибка — тогда уже выбрасываем исключение
            if (!responseMessage.IsSuccessStatusCode)
            {
                var errorBody = await responseMessage.Content.ReadAsStringAsync(ct);
                throw new Exception($"HH API Error ({responseMessage.StatusCode}): {errorBody}");
            }

            var response =
                await responseMessage.Content.ReadFromJsonAsync<HhNegotiationsResponse>(cancellationToken: ct);
            if (response == null || response.Items == null) break;

            totalPages = response.Pages;

            var tasks = response.Items.Select(async neg =>
            {
                Console.WriteLine(
                    $"[HH Debug] Candidate: {neg.Resume.LastName}, NegID: {neg.Id}, ResumeID: {neg.Resume.Id}");

                await semaphore.WaitAsync(ct);
                try
                {
                    string finalCoverLetter = neg.CoverLetter;
                    if (string.IsNullOrEmpty(finalCoverLetter))
                    {
                        finalCoverLetter = await GetFirstMessageAsync(token, neg.MessagesUrl, ct)
                                           ?? "Письмо отсутствует";
                    }

                    return new Candidate
                    {
                        // Используем ToString(), чтобы избежать проблем с большими числами в JS
                        NegotiationId = neg.Id.ToString(),
                        ResumeId = neg.Resume.Id.ToString(),
                        FullName = $"{neg.Resume.FirstName} {neg.Resume.LastName}",
                        DesiredPosition = neg.Resume.Title,
                        ResumeUrl = neg.Resume.AlternateUrl,
                        CoverLetter = finalCoverLetter,
                        BriefExperience = FormatExperience(neg.Resume.Experience)
                    };
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var pageCandidates = await Task.WhenAll(tasks);
            allCandidates.AddRange(pageCandidates);

            currentPage++;
        } while (currentPage < totalPages);

        return allCandidates;
    }

    public async Task<IEnumerable<HhAction>> GetAvailableActionsAsync(string accessToken, string negotiationId,
        CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(negotiationId)) return Enumerable.Empty<HhAction>();

        SetAuthHeader(accessToken);
        
        // ЛОГ ДЛЯ ТЕБЯ:
        Console.WriteLine($"[API TEST] Проверяю права для отклика: {negotiationId}");
    
        // Попробуй запросить данные самого отклика, а не действия
        var checkResp = await _httpClient.GetAsync($"negotiations/{negotiationId}", ct);
        
        if (checkResp.IsSuccessStatusCode)
        {
            var content = await checkResp.Content.ReadAsStringAsync(ct);
            // Мы увидим в каком "state" находится Даниил
            Console.WriteLine($"[DEBUG STATE] Данные отклика: {content}");
        }
        
        Console.WriteLine($"[API TEST] Доступ к самому отклику: {checkResp.StatusCode}");

        var response = await _httpClient.GetFromJsonAsync<HhNegotiation>($"negotiations/{negotiationId}", ct);

        return response?.Actions ?? Enumerable.Empty<HhAction>();
    }

    public async Task<(bool Success, string ErrorMessage)> ChangeCandidateStateAsync(
        string accessToken, string negotiationId, string actionId, CancellationToken ct = default)
    {
        SetAuthHeader(accessToken);

        // ВАЖНО: HH API требует PUT запрос на конкретный эндпоинт действия
        // Формат: negotiations/{action_id}/{negotiation_id}
        var url = $"negotiations/{actionId}/{negotiationId}";
    
        // Делаем пустой PUT запрос (тело запроса обычно не требуется для базовой смены статуса)
        var response = await _httpClient.PutAsync(url, null, ct);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"[HH Success] Кандидат {negotiationId} переведен в статус {actionId}");
            return (true, string.Empty);
        }

        var error = await response.Content.ReadAsStringAsync(ct);
        Console.WriteLine($"[HH Error] Не удалось изменить статус: {error}");
        return (false, error);
    }

    // В самом классе HeadHunterProvider:
    public async Task<IEnumerable<NegotiationCollection>> GetCollectionsAsync(string accessToken, string vacancyId,
        CancellationToken ct = default)
    {
        SetAuthHeader(accessToken);

        // Запрос коллекций для конкретной вакансии
        var url = $"negotiations?vacancy_id={vacancyId}";
        var response = await _httpClient.GetFromJsonAsync<HhCollectionsResponse>(url, ct);

        return response?.Collections ?? Enumerable.Empty<NegotiationCollection>();
    }

    private async Task<string?> GetFirstMessageAsync(string token, string messagesUrl, CancellationToken ct)
    {
        // Правило из ТЗ: берем первое релевантное сообщение из API переписки
        var response = await _httpClient.GetFromJsonAsync<HhMessagesResponse>(messagesUrl, ct);
        return response?.Items?.FirstOrDefault(m => m.Author.Role == "candidate")?.Text;
    }

    private string FormatExperience(IEnumerable<HhExperience>? experiences)
    {
        if (experiences == null) return "Нет данных об опыте";

        // ТЗ 3.1: Опыт в сокращённом виде (позиция, компания, стаж)
        return string.Join("; ", experiences.Select(e => $"{e.Position} в {e.Company}"));
    }

    // Остальные методы (GetVacancyDetailsAsync, ChangeCandidateStateAsync) реализуются аналогично


    public async Task<string> ExchangeCodeForTokenAsync(string code, CancellationToken ct = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "client_id", _options.ClientId }, // Из конфига
            { "client_secret", _options.ClientSecret }, // Из конфига
            { "code", code },
            { "redirect_uri", _options.RedirectUri } // Из конфига
        };

        var content = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync("https://hh.ru/oauth/token", content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new Exception($"Ошибка обмена токена: {error}");
        }

        // HH вернет JSON, из которого нам нужен только access_token
        var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        return result.GetProperty("access_token").GetString() ?? string.Empty;
    }

    public async Task<HhUserResponse> GetUserInfoAsync(string accessToken, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.hh.ru/me");
    
        // 1. Очищаем старые заголовки (на всякий случай)
        request.Headers.UserAgent.Clear();

        // 2. Устанавливаем "правильный" заголовок
        // ВАЖНО: Напиши здесь реально уникальное имя и свою почту
        var appName = "HHRankAnalyzer-Suleymenov"; // Уникальное имя
        var appVersion = "1.0";
        var contact = "suleymenov.janserik@gmail.com"; // Твоя реальная почта

        request.Headers.Add("User-Agent", $"{appName}/{appVersion} ({contact})");

        // 3. Добавляем авторизацию
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("Ошибка HH API: {Error}", errorContent);
            throw new HttpRequestException($"HH API error: {response.StatusCode}");
        }

        return await response.Content.ReadFromJsonAsync<HhUserResponse>(cancellationToken: ct) 
               ?? throw new Exception("Failed to parse user info");
    }

    private async Task<string?> GetCurrentEmployerIdAsync(string accessToken, CancellationToken ct)
    {
        SetAuthHeader(accessToken);
        var response = await _httpClient.GetFromJsonAsync<HhMeResponse>("me", ct);

        // Если аккаунт не привязан к работодателю, Employer будет null
        return response?.Employer?.Id;
    }

    private void SetAuthHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}