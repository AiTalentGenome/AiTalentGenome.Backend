namespace ResumeAnalyzer.WebApi.Models;

public record ChangeStateRequest(
    string NegotiationId,
    string ActionId
);