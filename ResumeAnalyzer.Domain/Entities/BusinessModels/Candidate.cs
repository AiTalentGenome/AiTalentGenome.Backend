using ResumeAnalyzer.Domain.Entities.AnalysisModels;

namespace ResumeAnalyzer.Domain.Entities.BusinessModels;

public class Candidate
{
    public string NegotiationId { get; set; } = string.Empty; // ID отклика в HH
    public string ResumeId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string DesiredPosition { get; set; } = string.Empty;
    public string ResumeUrl { get; set; } = string.Empty;
    
    // Данные для анализа
    public string BriefExperience { get; set; } = string.Empty;
    public string CoverLetter { get; set; } = string.Empty;

    // Результат анализа (может быть null, если еще не анализировали)
    public AnalysisResult? Analysis { get; set; }
    
    // Статус выбора в UI (чекбокс)
    public bool IsSelected { get; set; }
}