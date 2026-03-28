using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider.Models;

internal record HhNegotiationsResponse(
    [property: JsonPropertyName("items")] IEnumerable<HhNegotiation> Items,
    [property: JsonPropertyName("found")] int Found,
    [property: JsonPropertyName("pages")] int Pages,
    [property: JsonPropertyName("page")] int Page
);