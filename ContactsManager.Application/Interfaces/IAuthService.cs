using ContactsManager.Application.Common;
using ContactsManager.Application.DTOs.Auth;

namespace ContactsManager.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<UserDto>> RegisterAsync(RegisterDto dto);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
}
