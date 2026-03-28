using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Commands
{
    public record CancelAnalysisCommand() : IRequest;
}
