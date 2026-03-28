using Microsoft.EntityFrameworkCore;
using ResumeAnalyzer.Domain.Entities.Results;
using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Infrastructure.Data;

namespace ResumeAnalyzer.Infrastructure.Repositories;

public class AuditRepository(AppDbContext context) : IAuditRepository
{
    public async Task SaveRecordAsync(AuditRecord record, CancellationToken ct = default)
    {
        context.AuditRecords.Add(record);
        await context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<AuditRecord>> GetRecentRecordsAsync(int limit, CancellationToken ct = default)
    {
        return await context.AuditRecords
            .OrderByDescending(x => x.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);
    }
}