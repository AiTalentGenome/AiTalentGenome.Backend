using Microsoft.EntityFrameworkCore;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Domain.Entities.BusinessModels;
using ResumeAnalyzer.Domain.Entities.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Interfaces
{
    public interface IAppDbContext
    {
        public DbSet<AuditRecord> AuditRecords { get; }
        public DbSet<AnalysisAudit> AnalysisAudits { get; }

        public DbSet<Company> Companies { get; }

        public DbSet<AppUser> Users { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
