namespace ResumeAnalyzer.Domain.Entities.SaaS;

public class ClarifyQuestion
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = "Писать ничего не нужно — просто выберите вариант";
    public QuestionType Type { get; set; } // SingleChoice или MultiChoice
    public List<QuestionOption> Options { get; set; } = new();
    public bool IsRequired { get; set; } = true;
    
    public Dictionary<string, List<string>>? ShowIf { get; set; }
}