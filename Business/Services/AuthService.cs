using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class AuthService : IAuthService
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    public AuthService(SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IEmailService emailService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _emailService = emailService;
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
}
