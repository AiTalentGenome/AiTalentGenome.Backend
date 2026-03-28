using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Domain.Interfaces.Models;

public class HhEmployerInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty; // ВОТ ОН! ID компании в HH

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}