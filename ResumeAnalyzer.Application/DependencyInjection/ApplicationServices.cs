using Microsoft.Extensions.DependencyInjection;
using ResumeAnalyzer.Application.Services;

namespace ResumeAnalyzer.Application.DependencyInjection;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CandidateScoringService>();
        services.AddSingleton<AnalysisManager>();
        services.AddScoped<ClarifyService>();
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationServices).Assembly);
        });
        return services;
    }
}