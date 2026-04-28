using ContactsManager.Domain.Entities;

namespace ContactsManager.Domain.Interfaces;

public interface IContactRepository
{
    Task<(IEnumerable<Contact> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<Contact?> GetByIdAsync(int id);
    Task<Contact> CreateAsync(Contact contact);
    Task<Contact> UpdateAsync(Contact contact);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}
