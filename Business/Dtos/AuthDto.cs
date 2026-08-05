namespace Business.Dtos;
public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public bool IsPersistence { get; set; }
}
public class RegisterDto
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}