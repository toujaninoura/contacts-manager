namespace ContactsManager.Application.DTOs.Auth;

public record UserDto(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt
);
