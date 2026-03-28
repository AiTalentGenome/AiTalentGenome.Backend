using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using ResumeAnalyzer.Application.DTOs.Clarify;

namespace ResumeAnalyzer.Application.Notes.Clarify.Commands;

public record StartClarifyCommand(string VacancyId) : IRequest<ClarifyStep>;

public record SubmitAnswerCommand(
    string SessionId,
    UserAnswer Answer
) : IRequest<ClarifyStep>;