using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Business.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserService> _logger;
    public UserService(
        UserManager<AppUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IEmailService emailService,
        ILogger<UserService> logger
    )
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
        _logger = logger;
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
    public async Task<Result<string>> RequestUpdateCurrentEmailAsync(RequestUpdateEmailDto dto)
    {
        var newEmail = dto.NewEmail.Trim().ToLower();

        if (newEmail.Any(char.IsWhiteSpace))
            return Result<string>.Failure("Email cannot contain spaces.");

        var currentUserId = GetCurrentUserId();

        if (currentUserId == null)
            return Result<string>.Failure("UnAuthorized");

        var currentUser = await _userManager.FindByIdAsync(currentUserId);

        if (currentUser == null)
            return Result<string>.Failure("current user not found in db");

        if (string.Equals(currentUser.Email, dto.NewEmail, StringComparison.OrdinalIgnoreCase))
            return Result<string>.Failure("You cannot change with the same email");

        var isNewEmailExists = await _userManager.FindByEmailAsync(dto.NewEmail);

        if (isNewEmailExists != null)
            return Result<string>.Failure($"Email {dto.NewEmail} already taken ");

        currentUser.PendingEmail = dto.NewEmail;

        var result = await _userManager.UpdateAsync(currentUser);

        try
        {
            await _emailService.SendCodeAsync(currentUser, "Email Update", EmailPurposes.EMAIL_UPDATE, dto.NewEmail);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending confirmation code to new email.");
            return Result<string>.Failure("Failed to send confirmation code. Please try again later.");
        }
        if (result.Succeeded)
            return Result<string>.Success("Confirmation code sent to new email");

        return Result<string>.Failure(ServiceHelper.GetFirstError(result));

    }
    public async Task<Result<string>> UpdateEmailAsync(UpdateEmailDto dto)
    {
        var currentUserId = GetCurrentUserId();

        if (currentUserId == null)
            return Result<string>.Failure("UnAuthorized");

        var currentUser = await _userManager.FindByIdAsync(currentUserId);

        if (currentUser == null)
            return Result<string>.Failure("current user not found in db");

        var isValid = await _userManager.VerifyUserTokenAsync(currentUser, TokenOptions.DefaultEmailProvider, EmailPurposes.EMAIL_UPDATE, dto.Code);

        if (!isValid)
            return Result<string>.Failure("Invalid or expired code.");

        if (currentUser.PendingEmail == null)
            return Result<string>.Failure("No pending email was found");

        currentUser.Email = currentUser.PendingEmail;

        var updateResult = await _userManager.UpdateAsync(currentUser);

        if (!updateResult.Succeeded)
            return Result<string>.Failure(string.Join(",", updateResult.Errors.Select(e => e.Description)));

        currentUser.PendingEmail = null;
        await _userManager.UpdateAsync(currentUser);

        return Result<string>.Success("Email updated successfully");

    }
    public async Task<Result<string>> ResendUpdateEmailConfirmationCodeAsync()
    {
        var currentUserId = GetCurrentUserId();

        if (currentUserId == null)
            return Result<string>.Failure("Unauthorized");

        var currentUser = await _userManager.FindByIdAsync(currentUserId);

        if (currentUser == null)
            return Result<string>.Failure("User not found");

        if (string.IsNullOrEmpty(currentUser.PendingEmail))
            return Result<string>.Failure("No pending email update request found. Please request an email update again.");

        try
        {
            await _emailService.SendCodeAsync(currentUser, "Email Update", EmailPurposes.EMAIL_UPDATE, currentUser.PendingEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while resending confirmation code to new email.");
            return Result<string>.Failure("Failed to send confirmation code. Please try again later.");
        }

        return Result<string>.Success("Confirmation code resent to new email");
    }
}