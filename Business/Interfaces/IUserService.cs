namespace Business.Interfaces;

public interface IUserService
{
    string? GetCurrentUserId();
    string? GetCurrentUserRole();
    Task<Result<UserDto>> GetCurrentUserAsync();
}
