using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResumeAnalyzer.Application.Interfaces;
using ResumeAnalyzer.Domain.Configuration;
using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Infrastructure.Data;
using ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider;
using ResumeAnalyzer.Infrastructure.Providers.OllamaProvider;
using ResumeAnalyzer.Infrastructure.Repositories;

namespace ResumeAnalyzer.Infrastructure.DependencyInjection;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IOllamaProvider, OllamaProvider>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IHeadHunterProvider, HeadHunterProvider>();
        services.AddScoped<IAnalysisCacheRepository, AnalysisCacheRepository>();
        services.AddScoped<IQuestionRepository, JsonQuestionRepository>();
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }
}