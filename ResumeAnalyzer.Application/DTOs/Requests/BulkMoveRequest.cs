namespace ResumeAnalyzer.Application.DTOs.Requests;

public record BulkMoveRequest(List<string> NegotiationIds, string ActionId, string VacancyId, string CollectionId);