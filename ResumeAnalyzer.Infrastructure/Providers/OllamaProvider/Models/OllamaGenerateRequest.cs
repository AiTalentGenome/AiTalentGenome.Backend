namespace ResumeAnalyzer.Infrastructure.Providers.OllamaProvider.Models;

internal record OllamaGenerateRequest(
    string Model,
    string Prompt,
    bool Stream,
    string Format // Здесь мы передадим "json"
);