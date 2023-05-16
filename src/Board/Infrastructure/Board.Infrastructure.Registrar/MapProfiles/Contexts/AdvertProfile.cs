using AutoMapper;
using Board.Contracts.Contexts.Categories;
using Board.Contracts.Contexts.Adverts;
using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Registrar.MapProfiles.Contexts
{
    public class AdvertProfile : Profile
    {
        public AdvertProfile()
        {
            CreateMap<Advert, AdvertSummary>()
                .ForMember(a => a.ImagesId, map => map.MapFrom(s => s.AdvertImages.Select(a => a.ImageId)));
            CreateMap<Advert, AdvertDetails>()
                .ForMember(a => a.User, map => map.Ignore())
                .ForMember(a => a.ImagesId, map => map.MapFrom(s => s.AdvertImages.Select(a => a.ImageId)));
            CreateMap<AdvertAddRequest, Advert>()
                .ForMember(a => a.Id, map => map.Ignore())
                .ForMember(a => a.CreatedAt, map => map.MapFrom(d => DateTime.UtcNow))
                .ForMember(a => a.Category, map => map.Ignore())
                .ForMember(a => a.AdvertFavorites, map => map.Ignore())
                .ForMember(a => a.AdvertViews, map => map.Ignore())
                .ForMember(a => a.AdvertImages, map => map.Ignore())
                .ForMember(s => s.IsActive, map => map.MapFrom(a => true));
            CreateMap<AdvertUpdateRequest, Advert>()
                .ForMember(a => a.Id, map => map.Ignore())
                .ForMember(a => a.CreatedAt, map => map.Ignore())
                .ForMember(a => a.UserId, map => map.Ignore())
                .ForMember(a => a.Category, map => map.Ignore())
                .ForMember(a => a.AdvertFavorites, map => map.Ignore())
                .ForMember(a => a.AdvertViews, map => map.Ignore())
                .ForMember(a => a.AdvertImages, map => map.Ignore())
                .ForMember(s => s.IsActive, map => map.Ignore());

        }
    }
}
