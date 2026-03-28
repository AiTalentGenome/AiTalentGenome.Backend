using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Domain.Entities.AnalysisModels;

public record AnalysisResult(
    [property: JsonPropertyName("score")] int Score,
    [property: JsonPropertyName("rationale_ru")] string RationaleRu,
    [property: JsonPropertyName("flags")] AnalysisFlags Flags,
    [property: JsonPropertyName("extracted_fields")] ExtractedFields ExtractedFields
);