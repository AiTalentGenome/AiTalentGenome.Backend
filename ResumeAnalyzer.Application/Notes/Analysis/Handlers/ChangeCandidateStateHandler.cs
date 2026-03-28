using MediatR;
using ResumeAnalyzer.Application.Notes.Analysis.Commands;
using ResumeAnalyzer.Domain.Interfaces;

namespace ResumeAnalyzer.Application.Notes.Analysis.Handlers
{
    public class ChangeCandidateStateHandler(IHeadHunterProvider hhProvider)
    : IRequestHandler<ChangeCandidateStateCommand, (bool Success, string ErrorMessage)>
    {
        public async Task<(bool Success, string ErrorMessage)> Handle(ChangeCandidateStateCommand request, CancellationToken ct)
        {
            return await hhProvider.ChangeCandidateStateAsync(request.Token, request.NegotiationId, request.ActionId, ct);
        }
    }
}
