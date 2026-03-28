using MediatR;
using Microsoft.Extensions.Caching.Memory;
using ResumeAnalyzer.Application.DTOs.Clarify;
using ResumeAnalyzer.Application.Notes.Clarify.Commands;
using ResumeAnalyzer.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Clarify.Handlers;

public class StartClarifyHandler(ClarifyService clarifyService, IMemoryCache cache) : IRequestHandler<StartClarifyCommand, ClarifyStep>
{
    public async Task<ClarifyStep> Handle(StartClarifyCommand request, CancellationToken cancellationToken)
    {
        var sessionId = Guid.NewGuid().ToString();
        var initialAnswers = new List<UserAnswer>();

        cache.Set(sessionId, initialAnswers, TimeSpan.FromMinutes(30));

        var step = clarifyService.GetNextStep(initialAnswers);
        step.SessionId = sessionId;

        return step;
    }
}
