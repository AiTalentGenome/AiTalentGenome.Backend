using MediatR;
using ResumeAnalyzer.Domain.Entities.BusinessModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Queries
{
    public record GetCollectionsQuery(string Token, string VacancyId)
    : IRequest<IEnumerable<NegotiationCollection>>;
}
