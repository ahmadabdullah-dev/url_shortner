using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/user")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _userService.GetCurrentUserAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    [HttpPost("request-current-email-update")]
    public async Task<IActionResult> RequestUpdateCurrentEmail(RequestUpdateEmailDto dto)
    {
        var result = await _userService.RequestUpdateCurrentEmailAsync(dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    [HttpPatch("current-email")]
    public async Task<IActionResult> UpdateCurrentEmail(UpdateEmailDto dto)
    {
        var result = await _userService.UpdateEmailAsync(dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    [HttpPost("resend-current-email-update-confirmation-code")]
    public async Task<IActionResult> ResendUpdateEmailConfirmationCode()
    {
        var result = await _userService.ResendUpdateEmailConfirmationCodeAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    [HttpPut("current")]
    public async Task<IActionResult> UpdateCurrentUser(UpdateCurrentUserDto dto)
    {
        var result = await _userService.UpdateCurrentUserAsync(dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    [HttpPatch("current-username")]
    public async Task<IActionResult> UpdateCurrentUserName(UpdateUserNameDto dto)
    {
        var result = await _userService.UpdateUserNameAsync(dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
