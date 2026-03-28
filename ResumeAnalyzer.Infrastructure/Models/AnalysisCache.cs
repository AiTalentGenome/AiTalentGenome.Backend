namespace ResumeAnalyzer.Infrastructure.Models;

public class AnalysisCache
{
    public string Id { get; set; } = string.Empty; // Hash: ResumeId + VacancyId + CustomCriteria
    public string RawJsonResult { get; set; } = string.Empty; // Тот самый JSON от Ollama
    public DateTime CachedAt { get; set; }
}