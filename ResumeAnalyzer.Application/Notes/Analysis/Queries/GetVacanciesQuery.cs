using MediatR;
using ResumeAnalyzer.Domain.Entities.BusinessModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Queries
{
    public record GetVacanciesQuery(string Token) : IRequest<IEnumerable<Vacancy>>;
}
