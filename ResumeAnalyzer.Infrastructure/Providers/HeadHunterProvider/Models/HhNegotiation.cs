using System.Text.Json.Serialization;
using ResumeAnalyzer.Domain.Entities.Results;

namespace ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider.Models;

internal record HhNegotiation(
    [property: JsonPropertyName("id")] string Id, // ОБЯЗАТЕЛЬНО!
    [property: JsonPropertyName("resume")] HhResume Resume,
    [property: JsonPropertyName("messages_url")] string MessagesUrl,
    [property: JsonPropertyName("cover_letter")] string? CoverLetter,
    [property: JsonPropertyName("actions")] IEnumerable<HhAction>? Actions
);