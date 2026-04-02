using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResumeAnalyzer.Application.Interfaces;
using ResumeAnalyzer.Application.Notes.Auth.Commands;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Domain.Entities.BusinessModels;
using ResumeAnalyzer.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAnalyzer.Application.Notes.Auth.Handlers
{
    public class AuthExchangeHandler(
        IHeadHunterProvider hhProvider,
        IAppDbContext context,
        ILogger<AuthExchangeHandler> logger)
        : IRequestHandler<AuthExchangeCommand, AuthExchangeResult>
    {
        public async Task<AuthExchangeResult> Handle(AuthExchangeCommand request, CancellationToken ct)
        {
            var accessToken = await hhProvider.ExchangeCodeForTokenAsync(request.Code, ct);
            var hhProfile = await hhProvider.GetUserInfoAsync(accessToken, ct);

            if (hhProfile.Employer == null)
                throw new Exception("Аккаунт не привязан к компании.");

            var company = await context.Companies
                .FirstOrDefaultAsync(c => c.HhEmployerId == hhProfile.Employer.Id, ct);

            if (company == null)
            {
                company = new Company
                {
                    HhEmployerId = hhProfile.Employer.Id,
                    Name = hhProfile.Employer.Name,
                    IsActive = true,
                    SubscriptionExpiresAt = DateTime.UtcNow.AddDays(30) // Твой триал
                };
                context.Companies.Add(company);
            }
            else
            {
                company.Name = hhProfile.Employer.Name;
            }

            if (!company.IsActive || (company.SubscriptionExpiresAt < DateTime.UtcNow))
                return new AuthExchangeResult(accessToken, false, "Доступ заблокирован.");

            var user = await context.Users.FirstOrDefaultAsync(u => u.HhUserId == hhProfile.Id, ct);

            if (user == null)
            {
                context.Users.Add(new AppUser
                {
                    HhUserId = hhProfile.Id,
                    CompanyId = company.Id,
                    Email = hhProfile.Email,
                    FirstName = hhProfile.FirstName, 
                    LastName = hhProfile.LastName, 
                    MiddleName = hhProfile.MiddleName,
                    Position = "Менеджер" 
                });
            }
            else
            {
                user.Email = hhProfile.Email;
                user.FirstName = hhProfile.FirstName;
                user.LastName = hhProfile.LastName;
                user.MiddleName = hhProfile.MiddleName;
            }

            await context.SaveChangesAsync(ct);
            return new AuthExchangeResult(accessToken, true);
        }
    }
}