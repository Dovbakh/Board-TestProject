using AutoMapper;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Contexts.Users;
using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Registrar.MapProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserSummary>();
            CreateMap<User, UserDetails>();
            //CreateMap<Advert, AdvertSummary>();
            //CreateMap<Advert, AdvertDetails>();
            //CreateMap<AdvertAddRequest, Advert>()
            //    .ForMember(a => a.Id, map => map.Ignore())
            //    .ForMember(a => a.CreatedAt, map => map.MapFrom(d => DateTime.UtcNow))
            //    .ForMember(a => a.Category, map => map.Ignore())
            //    .ForMember(a => a.User, map => map.Ignore())
            //    .ForMember(a => a.AdvertImages, map => map.Ignore())
            //    .ForMember(s => s.isActive, map => map.MapFrom(a => true));
            //CreateMap<AdvertUpdateRequest, Advert>()
            //    .ForMember(a => a.Id, map => map.Ignore())
            //    .ForMember(a => a.CreatedAt, map => map.Ignore())
            //    .ForMember(a => a.UserId, map => map.Ignore())
            //    .ForMember(a => a.Category, map => map.Ignore())
            //    .ForMember(a => a.User, map => map.Ignore())
            //    .ForMember(a => a.AdvertImages, map => map.Ignore())
            //    .ForMember(s => s.isActive, map => map.Ignore());
        }
    }
}
