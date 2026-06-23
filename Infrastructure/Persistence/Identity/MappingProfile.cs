using AutoMapper;
using Domain.Entities;

namespace Persistence.Identity
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain AppUser -> Persistence AppIdentityUser
            CreateMap<AppUser, AppIdentityUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Identity ID'yi özel işle
                .ReverseMap();

            // Persistence AppIdentityUser -> Domain AppUser
            CreateMap<AppIdentityUser, AppUser>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id.ToString())));
        }
    }
}
