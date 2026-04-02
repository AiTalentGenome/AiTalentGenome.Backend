namespace ResumeAnalyzer.WebApi.Middleware;

public class HhAuthMiddleware
{
    private readonly RequestDelegate _next;

    public HhAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;

        // Проверяем все запросы к API, кроме обмена кода на токен (exchange)
        // и, возможно, страницы логина
        if (path.StartsWithSegments("/api") && !path.StartsWithSegments("/api/Auth/exchange"))
        {
            var token = context.Request.Cookies["hh_access_token"];

            if (string.IsNullOrEmpty(token))
            {
                // Если токена нет, а мы пытаемся зайти в защищенную зону
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Сессия истекла. Требуется авторизация." });
                return;
            }

            context.Items["HhToken"] = token;
        }

        await _next(context);
    }
}