namespace ResumeAnalyzer.Application.DTOs.Requests;

public record AnalysisRequestDto(
    string VacancyId,
    string CollectionId,
    int TopN,
    string CustomCriteria,
    List<ScoringRuleDto> PlusRules, 
    List<ScoringRuleDto> PenaltyRules);