using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using pbl3_web_api.DTO.Auth;
using pbl3_web_api.Entities;
using pbl3_web_api.Repository;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace pbl3_web_api.Service.impl;

public class AuthService : IAuthService
{
    private readonly IUserRepo _userRepo;
    private readonly IConfiguration _config;

    public AuthService(IUserRepo userRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _config = config;
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
    {
        if (await _userRepo.CheckUsernameExistsAsync(dto.Username))
            return new AuthResultDto { IsSuccess = false, Message = "Username đã tồn tại." };

        if (await _userRepo.CheckEmailExistsAsync(dto.Email))
            return new AuthResultDto { IsSuccess = false, Message = "Email đã tồn tại." };

        var user = new User
        {
            Username = dto.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            DateOfBirth = dto.DateOfBirth,
            Role = "Customer",
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var userId = await _userRepo.AddUserAsync(user);

        return new AuthResultDto 
        { 
            IsSuccess = true, 
            Message = "Đăng ký thành công.",
            UserId = userId
        };
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepo.GetUserByUsernameAsync(dto.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            return new AuthResultDto { IsSuccess = false, Message = "Sai tên đăng nhập hoặc mật khẩu." };

        if (!user.IsActive)
            return new AuthResultDto { IsSuccess = false, Message = "Tài khoản đã bị khóa." };

        var token = GenerateJwtToken(user);

        return new AuthResultDto
        {
            IsSuccess = true,
            Token = token,
            Role = user.Role,
            UserId = user.UserId,
            FullName = user.FullName
        };
    }

    [Authorize]
    public async Task ChangePasswordAsync(int id, string newPassword, string currentPassword)
    {
        var user = await _userRepo.GetUserByIdAsync(id);
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User không tồn tại.");
        }

        bool isCorrectPassword = BCrypt.Net.BCrypt.Verify(currentPassword, user.Password);
        if (!isCorrectPassword)
        {
            throw new Exception("Mật khẩu hiện tại không đúng.");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.Now;

        await _userRepo.UpdateUserAsync(user);
    }


    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name,           user.Username!),
            new Claim(ClaimTypes.Role,           user.Role!)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
