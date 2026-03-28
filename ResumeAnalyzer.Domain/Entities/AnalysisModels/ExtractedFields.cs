using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Domain.Entities.AnalysisModels;

public record ExtractedFields(
    [property: JsonPropertyName("desired_position")] string DesiredPosition,
    [property: JsonPropertyName("experience_summary")] string[] ExperienceSummary,
    [property: JsonPropertyName("cover_letter_used")] string CoverLetterUsed
);