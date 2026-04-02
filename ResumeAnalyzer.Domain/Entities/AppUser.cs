using ResumeAnalyzer.Domain.Entities.BusinessModels;

namespace ResumeAnalyzer.Domain.Entities;

public class AppUser
{
    public Guid Id { get; set; }
    public string HhUserId { get; set; } 
    public string Email { get; set; }
    
    // ДОБАВЛЯЕМ ДЛЯ МАКЕТА
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    
    // Это поле юзер заполнит сам в твоем UI
    public string? Position { get; set; } 

    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
}