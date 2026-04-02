using MediatR;
using ResumeAnalyzer.Application.Notes.Auth.Queries;
using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Domain.Interfaces.Models;

namespace ResumeAnalyzer.Application.Notes.Auth.Handlers
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
