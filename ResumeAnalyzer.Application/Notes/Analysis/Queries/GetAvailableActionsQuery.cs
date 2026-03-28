using MediatR;
using ResumeAnalyzer.Domain.Entities.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Queries
{
    public record GetAvailableActionsQuery(string Token, string NegotiationId) : IRequest<IEnumerable<HhAction>>;
}
