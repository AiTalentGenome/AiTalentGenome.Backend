using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider.Models;

internal record HhExperience(
    [property: JsonPropertyName("position")] string Position,
    [property: JsonPropertyName("company")] string Company
);