namespace ResumeAnalyzer.Domain.Entities.SaaS;

public class EvaluationRubric
{
    public string VacancyId { get; set; } = string.Empty;
    public List<ScoringCriterion> MustHave { get; set; } = new();
    public List<ScoringCriterion> NiceToHave { get; set; } = new();
    public List<ScoringCriterion> Penalties { get; set; } = new();
    public List<DealBreaker> DealBreakers { get; set; } = new();
    public int TopK { get; set; } = 10;
}