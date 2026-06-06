using pbl3_web_api.DTO.Auth;

namespace pbl3_web_api.Service;

public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(RegisterDto dto);
    Task<AuthResultDto> LoginAsync(LoginDto dto);
    Task ChangePasswordAsync(int id, string newPassword, string currentPassword);
}
