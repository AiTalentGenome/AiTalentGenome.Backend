namespace ResumeAnalyzer.Domain.Entities.Results;

public class AuditRecord
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string VacancyId { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty; // Например: "Перевод на этап Тестовое"
    public string ResultStatus { get; set; } = string.Empty; // "OK" или "Error"
    public string Message { get; set; } = string.Empty; // Детали ошибки
    public string NegotiationIds { get; set; } = string.Empty; // Список ID через запятую
}