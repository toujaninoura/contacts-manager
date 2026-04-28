namespace ContactsManager.Application.DTOs.Contacts;

public record ContactDto(
    int Id,
    string Nom,
    string Prenom,
    string Email,
    string? Telephone,
    int UserId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
