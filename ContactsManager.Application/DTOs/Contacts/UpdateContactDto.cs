namespace ContactsManager.Application.DTOs.Contacts;

public record UpdateContactDto(
    string Nom,
    string Prenom,
    string Email,
    string? Telephone
);
