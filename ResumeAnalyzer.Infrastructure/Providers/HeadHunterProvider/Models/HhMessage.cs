using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider.Models;

internal record HhMessage(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("created_at")] string CreatedAt, // Меняем DateTime на string
    [property: JsonPropertyName("author")] HhAuthor Author
);