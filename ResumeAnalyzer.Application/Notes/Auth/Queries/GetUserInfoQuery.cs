using MediatR;
using ResumeAnalyzer.Domain.Interfaces.Models;

namespace ResumeAnalyzer.Application.Notes.Auth.Queries
{
    public record GetUserInfoQuery(string Token) : IRequest<HhUserResponse>;
}
