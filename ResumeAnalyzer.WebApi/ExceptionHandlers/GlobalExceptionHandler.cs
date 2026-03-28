using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ResumeAnalyzer.WebApi.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Произошла необработанная ошибка: {Message}", exception.Message);

        var (statusCode, message) = exception switch
        {
            // Обрабатываем ошибки от HeadHunter
            HttpRequestException httpEx when httpEx.StatusCode == HttpStatusCode.Forbidden || httpEx.StatusCode == HttpStatusCode.Unauthorized 
                => (StatusCodes.Status401Unauthorized, "Сессия HeadHunter истекла или доступ запрещен. Пожалуйста, войдите снова."),
            
            HttpRequestException httpEx when httpEx.StatusCode == HttpStatusCode.BadRequest 
                => (StatusCodes.Status400BadRequest, "HeadHunter отклонил запрос. Проверьте настройки User-Agent или валидность токена."),
            
            _ => (StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера. Мы уже работаем над этим.")
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = "Ошибка API",
            Detail = message,
            Instance = httpContext.Request.Path
        }, cancellationToken);

        return true;
    }
}