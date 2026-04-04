using System;

namespace pbl3_web_api.DTO.Auth;

public class RegisterDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}

public class LoginDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class AuthResultDto
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    // Token details
    public string? Token { get; set; }
    public string? Role { get; set; }
    public int? UserId { get; set; }
    public string? FullName { get; set; }
}
