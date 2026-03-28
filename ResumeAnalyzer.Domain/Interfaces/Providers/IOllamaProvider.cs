using ResumeAnalyzer.Domain.Entities.AnalysisModels;

namespace ResumeAnalyzer.Domain.Interfaces;

public interface IOllamaProvider
{
    // Принимает подготовленный текст промпта и возвращает структурированный объект
    Task<AnalysisResult> AnalyzeResumeAsync(string prompt, CancellationToken ct = default);
}