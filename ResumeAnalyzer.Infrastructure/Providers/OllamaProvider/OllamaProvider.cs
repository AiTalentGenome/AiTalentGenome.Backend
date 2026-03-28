using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ResumeAnalyzer.Domain.Configuration;
using ResumeAnalyzer.Domain.Entities.AnalysisModels;
using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Infrastructure.Providers.OllamaProvider.Models;

namespace ResumeAnalyzer.Infrastructure.Providers.OllamaProvider;

public class OllamaProvider(HttpClient httpClient, IOptions<OllamaOptions> options) : IOllamaProvider
{
    private readonly OllamaOptions _options = options.Value;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true, // Важно!
        AllowTrailingCommas = true
    };

    public async Task<AnalysisResult?> AnalyzeResumeAsync(string prompt, CancellationToken ct = default)
    {
        var endpoint = $"{_options.BaseUrl}/api/generate";
        
        var requestBody = new OllamaGenerateRequest(
            Model: "llama3",
            Prompt: prompt,
            Stream: false,
            Format: "json" // ТЗ 3.2: Обязателен структурированный JSON
        );

        // Отправляем запрос к твоей RTX 3060
        var response = await httpClient.PostAsJsonAsync(endpoint, requestBody, ct);
        response.EnsureSuccessStatusCode();

        // Читаем ответ. Ollama возвращает JSON, где сам текст лежит в поле "response"
        var rawResponse = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: ct);
        var jsonString = rawResponse?.RootElement.GetProperty("response").GetString();

        // ВАЖНО: Выведи это в консоль!
        Console.WriteLine("--- RAW OLLAMA RESPONSE ---");
        Console.WriteLine(jsonString);
        Console.WriteLine("---------------------------");

        if (string.IsNullOrWhiteSpace(jsonString)) return null;

        // Очистка от Markdown (если Llama добавила ```json)
        jsonString = jsonString.Replace("```json", "").Replace("```", "").Trim();

        try
        {
            return JsonSerializer.Deserialize<AnalysisResult>(jsonString, _jsonOptions);
        }
        catch
        {
            return null; // Если не смогли распарсить — вернем null
        }
    }
}