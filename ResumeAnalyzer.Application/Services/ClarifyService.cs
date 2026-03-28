using ResumeAnalyzer.Application.DTOs.Clarify;
using ResumeAnalyzer.Domain.Entities.SaaS;
using ResumeAnalyzer.Domain.Interfaces;

namespace ResumeAnalyzer.Application.Services;

public class ClarifyService(IQuestionRepository repo)
{
    public ClarifyStep GetNextStep(List<UserAnswer> answers)
    {
        // 1. ПРИОРИТЕТ 1: Уровень позиции (Middle/Senior/Lead)
        if (!answers.Any(a => a.QuestionId == "position_level"))
        {
            return new ClarifyStep { NextQuestion = repo.GetById("position_level"), Done = false };
        }

        // 2. ПРИОРИТЕТ 2: Если Senior/Lead — спрашиваем про управление
        var levelAnswer = answers.First(a => a.QuestionId == "position_level");
        var level = levelAnswer.SelectedOptionIds.FirstOrDefault(); // Исправлено имя поля

        if ((level == "senior" || level == "lead") && !answers.Any(a => a.QuestionId == "management_style"))
        {
            return new ClarifyStep { NextQuestion = repo.GetById("management_style"), Done = false };
        }

        // 3. ПРИОРИТЕТ 3: Стек (Must-have)
        if (!answers.Any(a => a.QuestionId == "must_have_stack"))
        {
            return new ClarifyStep { NextQuestion = repo.GetById("must_have_stack"), Done = false };
        }

        // 4. ПРОВЕРКА: Достаточно ли данных?
        if (HasEnoughData(answers))
        {
            return new ClarifyStep { Done = true, Rubric = GenerateRubric(answers) };
        }

        // 5. ДЕФОЛТ: Если не сработали спец. правила, берем следующий вопрос по порядку из репозитория
        var lastAnswer = answers.LastOrDefault();
        var nextQ = repo.GetNext(lastAnswer?.QuestionId);

        if (nextQ == null) // Вопросы кончились
            return new ClarifyStep { Done = true, Rubric = GenerateRubric(answers) };

        return new ClarifyStep { NextQuestion = nextQ, Done = false };
    }

    // Вспомогательный метод для получения названия опции по ID (для сборки рубрики)
    private string GetLabelById(string optionId)
    {
        return repo.GetAll()
            .SelectMany(q => q.Options)
            .FirstOrDefault(o => o.Id == optionId)?.Label ?? optionId;
    }

    // Нормализация весов (чтобы сумма была 100)
    private void NormalizeWeights(EvaluationRubric rubric)
    {
        var total = rubric.MustHave.Sum(x => x.Weight);
        if (total <= 0) return;
        foreach (var item in rubric.MustHave) item.Weight = (item.Weight * 100) / total;
    }

    private bool HasEnoughData(List<UserAnswer> answers)
    {
        // 1. Понят ли уровень (не пропущен ли вопрос)
        bool hasLevel = answers.Any(a => a.QuestionId == "position_level" && !a.Skipped);

        // 2. Выбрано ли минимум 3 навыка в стеке
        var stackAnswer = answers.FirstOrDefault(a => a.QuestionId == "must_have_stack");
        bool hasMinSkills = stackAnswer != null && !stackAnswer.Skipped && stackAnswer.SelectedOptionIds.Count >= 3;

        // 3. Ответили ли на стоп-факторы (даже если выбрали "пусто", это ответ)
        bool hasStopFactors = answers.Any(a => a.QuestionId == "stop_factors");

        // 4. Понятен ли фокус (Hands-on / Management)
        bool hasFocus = answers.Any(a => a.QuestionId == "management_style" && !a.Skipped);
        
        return hasLevel && hasMinSkills && hasStopFactors && hasFocus;
    }

    public EvaluationRubric GenerateRubric(List<UserAnswer> answers)
    {
        var rubric = new EvaluationRubric();

        foreach (var answer in answers.Where(a => !a.Skipped))
        {
            switch (answer.QuestionId)
            {
                case "position_level":
                    rubric.MustHave.Add(new ScoringCriterion
                    {
                        Name = $"Опыт уровня {GetLabelById(answer.SelectedOptionIds.First())}",
                        Priority = "high",
                        Weight = 40
                    });
                    break;

                case "must_have_stack":
                    foreach (var skillId in answer.SelectedOptionIds)
                    {
                        rubric.MustHave.Add(new ScoringCriterion
                        {
                            Name = $"Владение {GetLabelById(skillId)}",
                            Priority = "mid",
                            Weight = 20
                        });
                    }

                    break;

                case "management_style":
                    rubric.NiceToHave.Add(new ScoringCriterion
                    {
                        Name = $"Стиль работы: {GetLabelById(answer.SelectedOptionIds.First())}",
                        Priority = "mid",
                        Weight = 15
                    });
                    break;

                case "stop_factors":
                    foreach (var factorId in answer.SelectedOptionIds)
                    {
                        rubric.DealBreakers.Add(new DealBreaker
                        {
                            Name = GetLabelById(factorId),
                            Enabled = true
                        });
                    }

                    break;
            }
        }
        
        NormalizeWeights(rubric);

        return rubric;
    }
}