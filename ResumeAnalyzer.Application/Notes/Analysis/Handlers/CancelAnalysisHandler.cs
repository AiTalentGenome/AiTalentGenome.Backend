using MediatR;
using ResumeAnalyzer.Application.Notes.Analysis.Commands;
using ResumeAnalyzer.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Handlers
{
    public class CancelAnalysisHandler(AnalysisManager analysisManager)
    : IRequestHandler<CancelAnalysisCommand>
    {
        public async Task Handle(CancelAnalysisCommand request, CancellationToken ct)
        {
            analysisManager.Stop();
            await Task.CompletedTask;
        }
    }
}
