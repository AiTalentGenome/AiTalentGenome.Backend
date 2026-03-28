using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Domain.Entities.AnalysisModels;

public record AnalysisFlags(
    [property: JsonPropertyName("missing_cover_letter")] bool MissingCoverLetter,
    [property: JsonPropertyName("missing_experience")] bool MissingExperience
);