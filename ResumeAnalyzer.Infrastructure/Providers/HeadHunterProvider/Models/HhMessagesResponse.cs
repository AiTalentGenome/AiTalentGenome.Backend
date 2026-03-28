using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Infrastructure.Providers.HeadHunterProvider.Models;

// Корень ответа для списка сообщений
internal record HhMessagesResponse(
    [property: JsonPropertyName("items")] IEnumerable<HhMessage> Items
);