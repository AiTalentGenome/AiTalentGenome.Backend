using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Domain.Interfaces.Models;

public class HhUserResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("profile_url")]
    public string? ProfileUrl { get; set; }
    
    [JsonPropertyName("employer")]
    public HhEmployerInfo? Employer { get; set; }
}