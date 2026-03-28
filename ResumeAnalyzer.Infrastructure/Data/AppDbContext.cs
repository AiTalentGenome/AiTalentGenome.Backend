using Microsoft.EntityFrameworkCore;
using ResumeAnalyzer.Application.Interfaces;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Domain.Entities.BusinessModels;
using ResumeAnalyzer.Domain.Entities.Results;

namespace ResumeAnalyzer.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    // Таблица для аудита (ТЗ 3.4)
    public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();
    public DbSet<AnalysisAudit> AnalysisAudits => Set<AnalysisAudit>();

    public DbSet<Company> Companies => Set<Company>();
    
    public DbSet<AppUser> Users => Set<AppUser>();
    
    // Таблица для кэша (ТЗ 4.1)
    public DbSet<AnalysisCacheEntry> AnalysisCache => Set<AnalysisCacheEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка ключей и индексов
        modelBuilder.Entity<AnalysisCacheEntry>().HasKey(x => x.CacheKey);
        modelBuilder.Entity<AuditRecord>().HasKey(x => x.Id);
    }
}

// Вспомогательная модель для кэша
public class AnalysisCacheEntry
{
    public string CacheKey { get; set; } = string.Empty; // Hash: ResumeId + VacancyId
    public string JsonResult { get; set; } = string.Empty; // Результат от Ollama
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}