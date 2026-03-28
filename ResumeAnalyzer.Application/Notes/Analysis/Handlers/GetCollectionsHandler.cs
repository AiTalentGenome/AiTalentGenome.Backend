using MediatR;
using ResumeAnalyzer.Application.Notes.Analysis.Queries;
using ResumeAnalyzer.Domain.Entities.BusinessModels;
using ResumeAnalyzer.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Handlers
{
    public class GetCollectionsHandler(IHeadHunterProvider hhProvider)
    : IRequestHandler<GetCollectionsQuery, IEnumerable<NegotiationCollection>>
    {
        public async Task<IEnumerable<NegotiationCollection>> Handle(GetCollectionsQuery request, CancellationToken ct)
        {
            return await hhProvider.GetCollectionsAsync(request.Token, request.VacancyId, ct);
        }
    }
}
