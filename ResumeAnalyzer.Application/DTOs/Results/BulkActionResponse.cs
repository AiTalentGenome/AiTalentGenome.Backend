using ResumeAnalyzer.Application.DTOs.Errors;

namespace ResumeAnalyzer.Application.DTOs.Results;

public record BulkActionResponse(
    int TotalProcessed,
    int SuccessCount,
    int FailedCount,
    List<CandidateError> Errors
);