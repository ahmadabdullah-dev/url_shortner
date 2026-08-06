using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class AuthService : IAuthService
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IUserService _userService;
    private readonly ApplicationDbContext _dbContext;
    public AuthService(SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IUserService userService,
        ApplicationDbContext dbContext)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _emailService = emailService;
        _userService = userService;
        _dbContext = dbContext;
    }   
    public async Task<Result<string>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email.ToLower());

        if (user == null)
            return Result<string>.Failure("Invalid email or password");

        if (await _userManager.IsLockedOutAsync(user))
            return Result<string>.Failure("User is locked. Please reset the password or wait 3 Minute.");

        var loginResult = await _signInManager.PasswordSignInAsync(user, dto.Password, dto.IsPersistence, true);

        if (loginResult.IsLockedOut)
            return Result<string>.Failure("User is locked. Please reset the password or wait 3 Minute.");

        if (!loginResult.Succeeded)
            return Result<string>.Failure("Invalid email or password");

        if (user.LockoutEnd != null)
            await _userManager.SetLockoutEndDateAsync(user, null);

        return Result<string>.Success("Logged in successfully");
    }
    public async Task<Result<string>> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return Result<string>.Success("Logged out successfully");
    }
    public async Task<Result<string>> RegisterAsync(RegisterDto dto)
    {
        var userName = dto.UserName.Trim().ToLower();
        var email = dto.Email.Trim().ToLower();

        var newUser = new AppUser
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = false,
        };

        var registerResult = await _userManager.CreateAsync(newUser, dto.Password);
        if (!registerResult.Succeeded)
            return Result<string>.Failure(ServiceHelper.GetFirstError(registerResult));

        var roleResult = await _userManager.AddToRoleAsync(newUser, UserRoles.COSTUMER);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(newUser);
            return Result<string>.Failure(ServiceHelper.GetFirstError(roleResult));
        }
        try
        {
            await _emailService.SendCodeAsync(newUser, "Email Confirmation", EmailPurposes.EMAIL_CONFIRMATION);
        }
        catch (Exception ex)
        {
            await _userManager.DeleteAsync(newUser);
            return Result<string>.Failure(ex.Message);
        }
        return Result<string>.Success("Registered successfully.");
    }
    public async Task<Result<string>> ConfirmEmailAsync(ConfirmEmailDto dto)
    {
        var userId = _userService.GetCurrentUserId();

        if (userId == null)
            return Result<string>.Failure("Unauthorized");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Result<string>.Failure("Current user not found in db");

        if (await _userManager.IsEmailConfirmedAsync(user))
            return Result<string>.Failure("Email already confirmed");

        var isValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultEmailProvider, EmailPurposes.EMAIL_CONFIRMATION, dto.Code);

        if (!isValid)
            return Result<string>.Failure("Invalid or expired code.");

        user.EmailConfirmed = true;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
            return Result<string>.Failure(ServiceHelper.GetFirstError(updateResult));

        return Result<string>.Success("Email confirmed successfully.");
    }
    public async Task<Result<string>> ResendEmailConfirmationCodeAsync()
    {
        var userId = _userService.GetCurrentUserId();

        if (userId == null)
            return Result<string>.Failure("Unauthorized");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Result<string>.Failure("Current user not found in db");

        if (await _userManager.IsEmailConfirmedAsync(user))
            return Result<string>.Failure("Email already confirmed");

        try
        {
            await _emailService.SendCodeAsync(user, "Email Confirmation", EmailPurposes.EMAIL_CONFIRMATION);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(ex.Message);
        }

        return Result<string>.Success("Email Confirmation code has been resent successfully");

    }
    public async Task<Result<string>> ForgetPasswordAsync(ForgetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            return Result<string>.Failure("User not found");

        await _emailService.SendCodeAsync(user, "Reset Password", EmailPurposes.PASSWORD_RESET);

        return Result<string>.Success("Reset code sent successfully.");
    }
    public async Task<Result<string>> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            return Result<string>.Failure("Invalid or expired code.");

        var isValid = await _userManager.VerifyUserTokenAsync(
            user, TokenOptions.DefaultEmailProvider, EmailPurposes.PASSWORD_RESET, dto.Code);

        if (!isValid)
            return Result<string>.Failure("Invalid or expired code.");

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        var removePasswordResult = await _userManager.RemovePasswordAsync(user);

        if (!removePasswordResult.Succeeded)
            return Result<string>.Failure(ServiceHelper.GetFirstError(removePasswordResult));

        var addPasswordResult = await _userManager.AddPasswordAsync(user, dto.NewPassword);

        if (!addPasswordResult.Succeeded)
            return Result<string>.Failure(ServiceHelper.GetFirstError(addPasswordResult));

        await _userManager.ResetAccessFailedCountAsync(user);
        await _userManager.SetLockoutEndDateAsync(user, null);

        await transaction.CommitAsync();

        return Result<string>.Success("Password reset successfully.");
    }
}
