using AutoMapper;
using Identity.Contracts.Contexts.Users;
using Identity.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Identity.Infrastructure.Registrar.MapProfiles.Contexts
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserSummary>();
            CreateMap<User, UserDetails>();
            CreateMap<UserRegisterRequest, User>()
                .ForMember(a => a.RoleId, map => map.MapFrom(s => Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6")))
                .ForMember(a => a.UserName, map => map.MapFrom(s => s.Email))
                .ForMember(a => a.CreatedAt, map => map.MapFrom(d => DateTime.UtcNow))
                .ForMember(a => a.Name, map => map.Ignore())
                .ForMember(a => a.Address, map => map.Ignore())
                .ForMember(a => a.Role, map => map.Ignore())
                .ForMember(a => a.isActive, map => map.Ignore())
                .ForMember(a => a.Id, map => map.Ignore())
                .ForMember(a => a.NormalizedUserName, map => map.Ignore())
                .ForMember(a => a.NormalizedEmail, map => map.Ignore())
                .ForMember(a => a.EmailConfirmed, map => map.Ignore())
                .ForMember(a => a.PasswordHash, map => map.Ignore())
                .ForMember(a => a.SecurityStamp, map => map.Ignore())
                .ForMember(a => a.ConcurrencyStamp, map => map.Ignore())
                .ForMember(a => a.PhoneNumber, map => map.Ignore())
                .ForMember(a => a.PhoneNumberConfirmed, map => map.Ignore())
                .ForMember(a => a.TwoFactorEnabled, map => map.Ignore())
                .ForMember(a => a.LockoutEnd, map => map.Ignore())
                .ForMember(a => a.LockoutEnabled, map => map.Ignore())
                .ForMember(a => a.AccessFailedCount, map => map.Ignore());

        }
    }
}
