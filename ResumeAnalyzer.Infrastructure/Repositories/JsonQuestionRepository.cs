using System.Text.Json;
using ResumeAnalyzer.Domain.Entities.SaaS;
using ResumeAnalyzer.Domain.Interfaces;

namespace ResumeAnalyzer.Infrastructure.Repositories;

public class JsonQuestionRepository : IQuestionRepository
{
    private readonly List<ClarifyQuestion> _questions;

    public JsonQuestionRepository()
    {
        // В реальном проекте путь берется из конфигурации
        var jsonData = File.ReadAllText("Infrastructure/Data/questions.json");
        _questions = JsonSerializer.Deserialize<List<ClarifyQuestion>>(jsonData, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        }) ?? new();
    }

    public ClarifyQuestion GetById(string id) => _questions.FirstOrDefault(q => q.Id == id);
    
    public IEnumerable<ClarifyQuestion> GetAll() => _questions;

    public ClarifyQuestion GetNext(string currentId)
    {
        var index = _questions.FindIndex(q => q.Id == currentId);
        return (index >= 0 && index < _questions.Count - 1) ? _questions[index + 1] : null;
    }
}