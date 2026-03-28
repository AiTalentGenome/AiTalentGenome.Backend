using MediatR;
using ResumeAnalyzer.Application.DTOs.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Commands
{
    public record BulkMoveCommand(
    string Token,
    string VacancyId,
    string CollectionId,
    List<string> NegotiationIds,
    string ActionId
) : IRequest<BulkMoveResult>;
}
