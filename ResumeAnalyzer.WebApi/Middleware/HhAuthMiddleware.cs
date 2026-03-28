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
        // Проверяем только запросы к API (исключая логин/exchange)
        if (context.Request.Path.StartsWithSegments("/api/Analysis"))
        {
            var token = context.Request.Cookies["hh_access_token"];

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Сессия истекла. Требуется авторизация." });
                return;
            }

            // Добавляем токен в Items, чтобы контроллер мог его легко достать
            context.Items["HhToken"] = token;
        }

        await _next(context);
    }
}