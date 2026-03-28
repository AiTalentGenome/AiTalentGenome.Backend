using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider.Models;

public record HhEmployerInfo(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name
);