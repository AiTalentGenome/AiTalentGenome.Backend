using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeAnalyzer.Application.Notes.Auth.Commands;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Domain.Entities.BusinessModels;
using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Infrastructure.Data;
using ResumeAnalyzer.WebApi.Models;

namespace ResumeAnalyzer.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")] // Теперь путь будет api/Auth
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("exchange")]
    public async Task<IActionResult> Exchange([FromBody] ExchangeRequest request, CancellationToken ct)
    {
        try
        {
            var result = await mediator.Send(new AuthExchangeCommand(request.Code), ct);

            if (!result.IsActive)
                return StatusCode(403, result.ErrorMessage);

            SetAuthCookie(result.AccessToken);

            return Ok(new { message = "Успешная авторизация" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("hh_access_token");
        return Ok(new { message = "Сессия завершена" });
    }

    private void SetAuthCookie(string accessToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,    // Защита от XSS (JS не увидит токен)
            Secure = true,      // Только через HTTPS (в деве может потребоваться false, если нет SSL)
            SameSite = SameSiteMode.None, // Защита от CSRF
            Expires = DateTime.UtcNow.AddDays(7), // Время жизни
            Path = "/"          // Кука доступна для всех путей
        };
        
        Response.Cookies.Append("hh_access_token", accessToken, cookieOptions);
    }
}