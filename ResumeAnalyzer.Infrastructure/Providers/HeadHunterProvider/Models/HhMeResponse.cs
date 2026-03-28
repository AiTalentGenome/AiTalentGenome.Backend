using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider.Models;

public record HhMeResponse(
    [property: JsonPropertyName("first_name")] string FirstName,
    [property: JsonPropertyName("last_name")] string LastName,
    [property: JsonPropertyName("employer")] HhEmployerInfo? Employer
);