namespace ResumeAnalyzer.Domain.Entities.BusinessModels;

public class Company
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string HhEmployerId { get; set; } = string.Empty; 
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true; 
    
    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubscriptionExpiresAt { get; set; } = DateTime.UtcNow.AddMonths(1); // Подписка примерно будет длится месяц
    
    public string? CustomApiKey { get; set; }
    
    public ICollection<AppUser> Users { get; set; }
}