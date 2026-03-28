using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Auth.Commands
{
    public record AuthExchangeCommand(string Code) : IRequest<AuthExchangeResult>;
}
