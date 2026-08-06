using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;   
    }   
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        return result.IsSuccess ? Ok(result) : Unauthorized(result);
    }
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var result = await _authService.LogoutAsync();

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    [Authorize]
    [HttpPatch("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto dto)
    {
        var result = await _authService.ConfirmEmailAsync(dto);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    [Authorize]
    [HttpPost("resend-email-confirmation-code")]
    public async Task<IActionResult> ResendEmailConfirmationCode()
    {
        var result = await _authService.ResendEmailConfirmationCodeAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
