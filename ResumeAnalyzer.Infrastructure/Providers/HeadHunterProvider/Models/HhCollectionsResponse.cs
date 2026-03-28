using System.Text.Json.Serialization;
using ResumeAnalyzer.Domain.Entities.BusinessModels;

namespace ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider.Models;

internal record HhCollectionsResponse(
    [property: JsonPropertyName("collections")] IEnumerable<NegotiationCollection> Collections
);