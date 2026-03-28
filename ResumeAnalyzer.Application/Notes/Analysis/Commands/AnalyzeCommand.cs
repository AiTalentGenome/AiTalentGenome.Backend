using MediatR;
using ResumeAnalyzer.Application.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Commands
{
    public record AnalyzeCommand(string Token, ScoringRequest Request) : IRequest<AnalyzeResult>;
}
