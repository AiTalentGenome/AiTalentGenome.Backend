using MediatR;
using Microsoft.Extensions.Caching.Memory;
using ResumeAnalyzer.Application.DTOs.Clarify;
using ResumeAnalyzer.Application.Notes.Clarify.Commands;
using ResumeAnalyzer.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Clarify.Handlers
{
    // Application/Clarify/Handlers/SubmitAnswerHandler.cs
    public class SubmitAnswerHandler(ClarifyService clarifyService, IMemoryCache cache)
        : IRequestHandler<SubmitAnswerCommand, ClarifyStep>
    {
        public async Task<ClarifyStep> Handle(SubmitAnswerCommand request, CancellationToken ct)
        {
            if (!cache.TryGetValue(request.SessionId, out List<UserAnswer>? answers) || answers == null)
            {
                throw new KeyNotFoundException("Сессия не найдена или истекла");
            }

            var existing = answers.FirstOrDefault(a => a.QuestionId == request.Answer.QuestionId);
            if (existing != null) answers.Remove(existing);
            answers.Add(request.Answer);

            var nextStep = clarifyService.GetNextStep(answers);
            nextStep.SessionId = request.SessionId;

            if (!nextStep.Done)
                cache.Set(request.SessionId, answers, TimeSpan.FromMinutes(30));
            else
                cache.Remove(request.SessionId);

            return nextStep;
        }
    }
}
