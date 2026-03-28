using Microsoft.EntityFrameworkCore;
using ResumeAnalyzer.Application.DependencyInjection;
using ResumeAnalyzer.Application.Hubs;
using ResumeAnalyzer.Domain.Configuration;
using ResumeAnalyzer.Infrastructure.Data;
using ResumeAnalyzer.Infrastructure.DependencyInjection;
using ResumeAnalyzer.WebApi.ExceptionHandlers;
using ResumeAnalyzer.WebApi.Extensions;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// В Program.cs (Web API проект)
builder.Services.Configure<HhOptions>(builder.Configuration.GetSection("HH"));
builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection("Ollama"));

builder.Services.AddControllers();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.HandshakeTimeout = TimeSpan.FromSeconds(30); // Ждем дольше при подключении
    options.KeepAliveInterval = TimeSpan.FromSeconds(15); // Чаще проверяем пульс
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60); // Не рвем связь минуту, если сервер занят
});

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:8000", "http://localhost:3000") // Твой Next.js на 8000 порту
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // КРИТИЧНО для SignalR
        });
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", 
        rollingInterval: RollingInterval.Day, // Новый файл каждый день
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Это создаст таблицы, если их нет
}

app.UseRouting();

app.UseCors(myAllowSpecificOrigins);

if (app.Environment.IsDevelopment())
{
    // Генерирует JSON-файл с описанием всех эндпоинтов
    app.MapOpenApi(); 
    
    // Запускает Scalar UI по адресу /scalar-api
    app.MapScalarApiReference(options => 
    {
        options
            .WithTitle("Resume Analyzer API")
            .WithTheme(ScalarTheme.Moon) // Тёмная тема выглядит круто
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHhAuth();

app.UseExceptionHandler();

app.MapControllers();

app.MapHub<AnalysisProgressHub>("/hubs/progress");

app.Run();