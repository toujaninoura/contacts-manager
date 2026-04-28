namespace ContactsManager.Domain.Entities;

public class Contact
{
    public int Id { get; init; }
    public string Nom { get; init; } = string.Empty;
    public string Prenom { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Telephone { get; init; }
    public int UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public Contact() { }

    public Contact(string nom, string prenom, string email, string? telephone, int userId)
    {
        Nom = nom;
        Prenom = prenom;
        Email = email;
        Telephone = telephone;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Contact WithUpdate(string nom, string prenom, string email, string? telephone)
    {
        return new Contact
        {
            Id = Id,
            Nom = nom,
            Prenom = prenom,
            Email = email,
            Telephone = telephone,
            UserId = UserId,
            CreatedAt = CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
