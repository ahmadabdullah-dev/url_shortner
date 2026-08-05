using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities.Identity;

public class AppUser : IdentityUser
{
    public string? ImagePath { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Country { get; set; }
    public bool IsActive { get; set; } 
    public DateOnly? DateOfBirth { get; set; }
    public string? PendingEmail { get; set; } 
    public DateTime CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; } 
}