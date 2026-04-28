using AutoMapper;
using ContactsManager.Application.DTOs.Contacts;
using ContactsManager.Domain.Entities;

namespace ContactsManager.Application.Mappings;

public class ContactMappingProfile : Profile
{
    public ContactMappingProfile()
    {
        CreateMap<Contact, ContactDto>();
        CreateMap<CreateContactDto, Contact>()
            .ConstructUsing(dto => new Contact(dto.Nom, dto.Prenom, dto.Email, dto.Telephone, dto.UserId));
    }
}
