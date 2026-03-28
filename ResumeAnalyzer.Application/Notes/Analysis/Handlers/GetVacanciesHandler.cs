using MediatR;
using ResumeAnalyzer.Application.Notes.Analysis.Queries;
using ResumeAnalyzer.Domain.Entities.BusinessModels;
using ResumeAnalyzer.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Handlers
{
    public class GetVacanciesHandler(IHeadHunterProvider hhProvider)
    : IRequestHandler<GetVacanciesQuery, IEnumerable<Vacancy>>
    {
        public async Task<IEnumerable<Vacancy>> Handle(GetVacanciesQuery request, CancellationToken ct)
        {
            return await hhProvider.GetVacanciesAsync(request.Token, ct);
        }
    }
}
