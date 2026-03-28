using ResumeAnalyzer.Domain.Entities.BusinessModels;

namespace ResumeAnalyzer.Domain.Entities;

public class AppUser
{
    public Guid Id { get; set; }
    public string HhUserId { get; set; } // ID менеджера из HH
    public string Email { get; set; }
    public Guid CompanyId { get; set; }
    public Company Company { get; set; }
}