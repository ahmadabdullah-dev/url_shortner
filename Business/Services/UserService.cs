using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Business.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserService(
        UserManager<AppUser> userManager,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public string? GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
    public string? GetCurrentUserRole()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
    }
    public async Task<Result<UserDto>> GetCurrentUserAsync()
    {
        var userId = GetCurrentUserId();
        var role = GetCurrentUserRole();

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
            return Result<UserDto>.Failure("You must be logged in to perform this action.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Result<UserDto>.Failure("User not found!. It may have been removed or deactivated.");

        var userDto = new UserDto
        {
            UserName = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            Country = user.Country!,
            CreatedAt = user.CreatedAt.ToString(),
            BirthDate = user.DateOfBirth.ToString(),
            IsEmailConfirmed = user.EmailConfirmed,
            Role = role
        };
        return Result<UserDto>.Success(userDto);
    }
}