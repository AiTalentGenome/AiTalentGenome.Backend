using ResumeAnalyzer.Application.DTOs.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Commands
{
    public record AnalyzeResult(
    IEnumerable<CandidateResultDto>? Candidates = null,
    bool IsCancelled = false,
    string? ErrorMessage = null
);
}
