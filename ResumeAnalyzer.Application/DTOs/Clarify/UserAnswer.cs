namespace ResumeAnalyzer.Application.DTOs.Clarify;

public class UserAnswer
{
    public string QuestionId { get; set; } = string.Empty;
    
    // Даже для одиночного выбора шлем массив, как просит ТЗ
    public List<string> SelectedOptionIds { get; set; } = new();
    
    public bool Skipped { get; set; } = false;
}