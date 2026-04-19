using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pbl3_web_api.DTO.Auth;
using pbl3_web_api.Entities;
using pbl3_web_api.Service;
using System.Security.Claims;
using System.Threading.Tasks;

namespace pbl3_web_api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (!result.IsSuccess)
            return Conflict(new { message = result.Message });

        return StatusCode(201, new { message = result.Message, userId = result.UserId });
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (!result.IsSuccess)
            return Unauthorized(new { message = result.Message });

        return Ok(new
        {
            token = result.Token,
            role = result.Role,
            userId = result.UserId,
            fullName = result.FullName
        });
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return BadRequest();
        int id = int.Parse(userId);

        try
        {
            await _authService.ChangePasswordAsync(
                id,
                dto.newPassword,
                dto.currentPassword
            );

            return Ok(new { message = "Đổi mật khẩu thành công." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}