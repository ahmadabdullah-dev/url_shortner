namespace Business.Interfaces;

public interface IAuthService
{
    Task<Result<string>> LoginAsync(LoginDto dto);
    Task<Result<string>> LogoutAsync();

}
