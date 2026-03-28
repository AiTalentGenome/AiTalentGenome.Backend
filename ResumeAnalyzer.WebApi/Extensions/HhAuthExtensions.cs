using ResumeAnalyzer.WebApi.Middleware;

namespace ResumeAnalyzer.WebApi.Extensions;

public static class HhAuthExtensions
{
    /// <summary>
    /// Регистрирует кастомное Middleware для авторизации через HeadHunter Cookies.
    /// </summary>
    public static IApplicationBuilder UseHhAuth(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<HhAuthMiddleware>();
    }
}