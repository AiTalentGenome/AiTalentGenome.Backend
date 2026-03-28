using ResumeAnalyzer.Domain.Entities.Results;

namespace ResumeAnalyzer.Domain.Interfaces;

public interface IAuditRepository
{
    Task SaveRecordAsync(AuditRecord record, CancellationToken ct = default);
    Task<IEnumerable<AuditRecord>> GetRecentRecordsAsync(int limit, CancellationToken ct = default);
}