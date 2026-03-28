using ResumeAnalyzer.Domain.Entities.SaaS;

namespace ResumeAnalyzer.Application.DTOs.Clarify;

public class ClarifyStep
{
    public bool Done { get; set; }
    
    // Если Done == false, здесь будет вопрос
    public ClarifyQuestion? NextQuestion { get; set; }
    
    // Если Done == true, здесь будет итоговая рубрика
    public EvaluationRubric? Rubric { get; set; }
    
    // Примерное кол-во вопросов для прогресс-бара
    public int ApproxTotal { get; set; } = 5;
    
    public string? SessionId { get; set; }
}