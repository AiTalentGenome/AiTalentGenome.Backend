using MediatR;
using ResumeAnalyzer.Application.Notes.Analysis.Queries;
using ResumeAnalyzer.Domain.Entities.Results;
using ResumeAnalyzer.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Handlers
{
    public class GetAvailableActionsHandler(IHeadHunterProvider hhProvider)
    : IRequestHandler<GetAvailableActionsQuery, IEnumerable<HhAction>>
    {
        public async Task<IEnumerable<HhAction>> Handle(GetAvailableActionsQuery request, CancellationToken ct)
        {
            return await hhProvider.GetAvailableActionsAsync(request.Token, request.NegotiationId, ct);
        }
    }
}
