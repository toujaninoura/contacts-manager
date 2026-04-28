using AutoMapper;
using ContactsManager.Application.DTOs.Auth;
using ContactsManager.Domain.Entities;

namespace ContactsManager.Application.Mappings;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}
