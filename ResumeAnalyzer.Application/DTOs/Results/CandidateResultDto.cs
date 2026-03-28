namespace ResumeAnalyzer.Application.DTOs.Results;

public record CandidateResultDto(
    string NegotiationId,
    string FullName,
    int Score,
    string Rationale,
    string ResumeUrl,
    bool MissingCoverLetter
);