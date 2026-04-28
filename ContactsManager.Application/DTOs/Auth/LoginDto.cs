namespace ContactsManager.Application.DTOs.Auth;

public record LoginDto(
    string Email,
    string Password
);
