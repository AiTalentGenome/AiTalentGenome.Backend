namespace ResumeAnalyzer.Domain.Interfaces;

public interface IAnalysisCacheRepository
{
    Task<string?> GetCachedResultAsync(string cacheKey, CancellationToken ct);
    Task SaveToCacheAsync(string cacheKey, string jsonResult, CancellationToken ct);
}