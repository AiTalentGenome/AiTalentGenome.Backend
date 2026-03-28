using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider.Models;

internal record HhResume(
    [property: JsonPropertyName("id")] string Id, // ТЕПЕРЬ БУДЕТ РАБОТАТЬ
    [property: JsonPropertyName("first_name")] string FirstName,
    [property: JsonPropertyName("last_name")] string LastName,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("alternate_url")] string AlternateUrl,
    [property: JsonPropertyName("experience")] IEnumerable<HhExperience>? Experience
);