using ContactsManager.Domain.Entities;

namespace ContactsManager.Domain.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<bool> EmailExistsAsync(string email);
}
