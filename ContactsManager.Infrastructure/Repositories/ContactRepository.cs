using ContactsManager.Domain.Entities;
using ContactsManager.Domain.Interfaces;
using ContactsManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly ApplicationDbContext _context;

    public ContactRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Contact> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var query = _context.Contacts.AsNoTracking();

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.Nom)
            .ThenBy(c => c.Prenom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Contact?> GetByIdAsync(int id)
    {
        return await _context.Contacts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Contact> CreateAsync(Contact contact)
    {
        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task<Contact> UpdateAsync(Contact contact)
    {
        _context.Contacts.Update(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task DeleteAsync(int id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact is not null)
        {
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Contacts
            .AsNoTracking()
            .AnyAsync(c => c.Id == id);
    }
}
