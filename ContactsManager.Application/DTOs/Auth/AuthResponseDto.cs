namespace ContactsManager.Application.DTOs.Auth;

public record AuthResponseDto(
    string Token,
    DateTime ExpiresAt,
    UserDto User
);
