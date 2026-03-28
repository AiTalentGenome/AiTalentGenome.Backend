using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Domain.Entities.Results;

public record HhAction(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name
);