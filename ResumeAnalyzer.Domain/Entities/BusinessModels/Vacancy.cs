namespace ResumeAnalyzer.Domain.Entities.BusinessModels;

public class Vacancy
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; // Полный текст для промпта
}