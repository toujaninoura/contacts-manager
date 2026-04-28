namespace ContactsManager.Application.DTOs.Contacts;

public record CreateContactDto(
    string Nom,
    string Prenom,
    string Email,
    string? Telephone,
    int UserId
);
