using ResumeAnalyzer.Application.DTOs.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.DTOs.Results
{
    public record BulkMoveResult(
    int SuccessCount,
    int FailedCount,
    List<CandidateError> Errors
);
}
