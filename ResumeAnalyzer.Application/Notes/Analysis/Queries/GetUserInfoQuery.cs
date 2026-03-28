using MediatR;
using ResumeAnalyzer.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Queries
{
    public record GetUserInfoQuery(string Token) : IRequest<HhUserResponse>;
}
