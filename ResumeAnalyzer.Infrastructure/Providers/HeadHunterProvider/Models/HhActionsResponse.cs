using System.Text.Json.Serialization;
using ResumeAnalyzer.Domain.Entities.Results;

namespace ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider.Models;

internal record HhActionsResponse(
    [property: JsonPropertyName("items")] IEnumerable<HhAction> Items
);