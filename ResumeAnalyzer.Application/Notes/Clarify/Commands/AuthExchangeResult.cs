using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Clarify.Commands
{
    public record AuthExchangeResult(
    string AccessToken,
    bool IsActive,
    string? ErrorMessage = null
);
}
