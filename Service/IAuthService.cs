using pbl3_web_api.DTO.Auth;
using System.Threading.Tasks;

namespace pbl3_web_api.Service;

public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(RegisterDto dto);
    Task<AuthResultDto> LoginAsync(LoginDto dto);
}
