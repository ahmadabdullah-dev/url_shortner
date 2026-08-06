namespace Business.Dtos;

public class UserDto
{
    public string? ImageUrl { get; set; }
    public string UserName { get; set; } = null!;
    public string? FirstName { get; set; } 
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public bool IsActive { get; set; } 
    public int RewardsPoint { get; set; } 
    public string Email { get; set; } = null!;
    public bool IsEmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; } 
    public string? Country { get; set; } 
    public string? BirthDate { get; set; } 
    public string CreatedAt { get; set; } = null!;
    public string Role { get; set; } = null!;
   
}
public class RequestUpdateEmailDto
{
    public required string NewEmail { get; set; }
}
public class UpdateEmailDto
{
    public required string Code { get; set; }
}