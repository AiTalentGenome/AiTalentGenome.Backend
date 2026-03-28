using ResumeAnalyzer.Domain.Entities.BusinessModels;

namespace ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider.Models;

internal record HhVacanciesResponse(IEnumerable<Vacancy> Items);