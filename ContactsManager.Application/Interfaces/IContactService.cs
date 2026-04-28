using ContactsManager.Application.Common;
using ContactsManager.Application.DTOs.Contacts;

namespace ContactsManager.Application.Interfaces;

public interface IContactService
{
    Task<ApiResponse<PagedResult<ContactDto>>> GetAllAsync(int page, int pageSize);
    Task<ApiResponse<ContactDto>> GetByIdAsync(int id);
    Task<ApiResponse<ContactDto>> CreateAsync(CreateContactDto dto);
    Task<ApiResponse<ContactDto>> UpdateAsync(int id, UpdateContactDto dto);
    Task DeleteAsync(int id);
}
