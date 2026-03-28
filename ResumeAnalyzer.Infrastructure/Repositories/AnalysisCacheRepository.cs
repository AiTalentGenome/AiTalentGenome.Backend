using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Infrastructure.Data;

namespace ResumeAnalyzer.Infrastructure.Repositories;

public class AnalysisCacheRepository(AppDbContext context) : IAnalysisCacheRepository
{
    public async Task<string?> GetCachedResultAsync(string cacheKey, CancellationToken ct)
    {
        var entry = await context.AnalysisCache.FindAsync([cacheKey], ct);
        return entry?.JsonResult;
    }

    public async Task SaveToCacheAsync(string cacheKey, string jsonResult, CancellationToken ct)
    {
        context.AnalysisCache.Add(new AnalysisCacheEntry 
        { 
            CacheKey = cacheKey, 
            JsonResult = jsonResult, 
            CreatedAt = DateTime.UtcNow 
        });
        await context.SaveChangesAsync(ct);
    }
}