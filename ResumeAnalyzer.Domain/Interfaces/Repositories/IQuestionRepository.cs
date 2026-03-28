using ResumeAnalyzer.Domain.Entities.SaaS;

namespace ResumeAnalyzer.Domain.Interfaces;

public interface IQuestionRepository
{
    ClarifyQuestion GetById(string id);
    IEnumerable<ClarifyQuestion> GetAll();
    ClarifyQuestion GetNext(string currentId);
}