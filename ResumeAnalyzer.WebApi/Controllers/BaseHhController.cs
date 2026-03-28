using Microsoft.AspNetCore.Mvc;

namespace ResumeAnalyzer.WebApi.Controllers;

[ApiController]
public abstract class BaseHhController : ControllerBase
{
    // Это свойство заменяет всю ту длинную строку с Request.Headers...
    protected string HhToken => HttpContext.Items["HhToken"] as string 
                                ?? throw new UnauthorizedAccessException("Токен HeadHunter не найден в контексте запроса");
}