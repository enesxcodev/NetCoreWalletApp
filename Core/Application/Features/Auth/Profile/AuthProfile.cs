using AutoMapper;
using Application.Features.Auth.Commands;
using Domain.Entities;

namespace Application.Features.Auth.Profile
{
    public class AuthProfile : AutoMapper.Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterCreateCommand, AppUser>()                
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))                
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}