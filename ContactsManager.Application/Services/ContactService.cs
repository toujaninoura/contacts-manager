using AutoMapper;
using ContactsManager.Application.Common;
using ContactsManager.Application.DTOs.Contacts;
using ContactsManager.Application.Interfaces;
using ContactsManager.Domain.Entities;
using ContactsManager.Domain.Exceptions;
using ContactsManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ContactsManager.Application.Services;

public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ContactService> _logger;

    public ContactService(
        IContactRepository contactRepository,
        IMapper mapper,
        ILogger<ContactService> logger)
    {
        _contactRepository = contactRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<ContactDto>>> GetAllAsync(int page, int pageSize)
    {
        _logger.LogInformation("Fetching contacts page={Page} pageSize={PageSize}", page, pageSize);

        var (items, totalCount) = await _contactRepository.GetAllAsync(page, pageSize);

        var dtos = items.Select(c => _mapper.Map<ContactDto>(c));

        var pagedResult = new PagedResult<ContactDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponse<PagedResult<ContactDto>>.Ok(pagedResult);
    }

    public async Task<ApiResponse<ContactDto>> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching contact id={Id}", id);

        var contact = await _contactRepository.GetByIdAsync(id);
        if (contact is null)
        {
            _logger.LogWarning("Contact id={Id} not found", id);
            throw new NotFoundException(nameof(Contact), id);
        }

        var dto = _mapper.Map<ContactDto>(contact);
        return ApiResponse<ContactDto>.Ok(dto);
    }

    public async Task<ApiResponse<ContactDto>> CreateAsync(CreateContactDto dto)
    {
        _logger.LogInformation("Creating contact email={Email}", dto.Email);

        var emailExists = await _contactRepository.EmailExistsAsync(dto.Email, null);
        if (emailExists)
        {
            _logger.LogWarning("Create contact failed: email already in use - {Email}", dto.Email);
            return ApiResponse<ContactDto>.Fail("Email deja utilise.");
        }

        var contact = _mapper.Map<Contact>(dto);
        var created = await _contactRepository.CreateAsync(contact);

        _logger.LogInformation("Contact created id={Id}", created.Id);

        var contactDto = _mapper.Map<ContactDto>(created);
        return ApiResponse<ContactDto>.Ok(contactDto, "Contact cree avec succes.");
    }

    public async Task<ApiResponse<ContactDto>> UpdateAsync(int id, UpdateContactDto dto)
    {
        _logger.LogInformation("Updating contact id={Id}", id);

        var existing = await _contactRepository.GetByIdAsync(id);
        if (existing is null)
        {
            _logger.LogWarning("Contact id={Id} not found for update", id);
            throw new NotFoundException(nameof(Contact), id);
        }

        var emailExists = await _contactRepository.EmailExistsAsync(dto.Email, id);
        if (emailExists)
        {
            _logger.LogWarning("Update contact id={Id} failed: email already in use - {Email}", id, dto.Email);
            return ApiResponse<ContactDto>.Fail("Email deja utilise.");
        }

        var updated = existing.WithUpdate(dto.Nom, dto.Prenom, dto.Email, dto.Telephone);
        var saved = await _contactRepository.UpdateAsync(updated);

        _logger.LogInformation("Contact id={Id} updated successfully", id);

        var contactDto = _mapper.Map<ContactDto>(saved);
        return ApiResponse<ContactDto>.Ok(contactDto, "Contact mis a jour avec succes.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting contact id={Id}", id);

        var exists = await _contactRepository.ExistsAsync(id);
        if (!exists)
        {
            _logger.LogWarning("Contact id={Id} not found for deletion", id);
            throw new NotFoundException(nameof(Contact), id);
        }

        await _contactRepository.DeleteAsync(id);
        _logger.LogInformation("Contact id={Id} deleted successfully", id);
        return ApiResponse<bool>.Ok(true, "Contact supprime avec succes.");
    }
}
