using MediatR;
using ResumeAnalyzer.Application.Notes.Analysis.Queries;
using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Handlers
{
    public class GetUserInfoHandler(IHeadHunterProvider hhProvider)
    : IRequestHandler<GetUserInfoQuery, HhUserResponse>
    {
        public async Task<HhUserResponse> Handle(GetUserInfoQuery request, CancellationToken ct)
        {
            // Просто перенаправляем вызов провайдеру
            return await hhProvider.GetUserInfoAsync(request.Token, ct);
        }
    }
}
