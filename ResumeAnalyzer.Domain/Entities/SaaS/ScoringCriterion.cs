namespace ResumeAnalyzer.Domain.Entities.SaaS;

public class ScoringCriterion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public double Weight { get; set; } // Базовый вес (напр. 20.0)
    
    // Приоритет для UI (Экраны 2A и 2C)
    public string Priority { get; set; } = "mid";
}