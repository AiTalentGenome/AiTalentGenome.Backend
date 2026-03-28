using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Analysis.Commands
{
    public record ChangeCandidateStateCommand(string Token, string NegotiationId, string ActionId) : IRequest<(bool Success, string ErrorMessage)>;
}
