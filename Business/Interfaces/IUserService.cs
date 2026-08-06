namespace Business.Interfaces;

public interface IUserService
{
    string? GetCurrentUserId();
    string? GetCurrentUserRole();
    Task<Result<UserDto>> GetCurrentUserAsync();
    Task<Result<string>> RequestUpdateCurrentEmailAsync(RequestUpdateEmailDto dto);
    Task<Result<string>> UpdateEmailAsync(UpdateEmailDto dto);
    Task<Result<string>> ResendUpdateEmailConfirmationCodeAsync();
}
