namespace ResumeAnalyzer.Domain.Entities.Results;

public class AnalysisAudit
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string VacancyId { get; set; } = string.Empty;
    public string CollectionId { get; set; } = string.Empty;
    public string ActionId { get; set; } = string.Empty;
    
    // Список ID через запятую
    public string NegotiationIds { get; set; } = string.Empty; 
    
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    
    // Полный лог ошибок в формате JSON
    public string Details { get; set; } = string.Empty;
}