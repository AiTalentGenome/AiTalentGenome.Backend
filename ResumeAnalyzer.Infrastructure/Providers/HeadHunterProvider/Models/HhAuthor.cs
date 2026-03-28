using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider.Models;

internal record HhAuthor(
    [property: JsonPropertyName("role")] string Role
);