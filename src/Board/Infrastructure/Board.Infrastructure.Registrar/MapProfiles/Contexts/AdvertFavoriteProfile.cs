using AutoMapper;
using Board.Contracts.Contexts.AdvertFavorites;
using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Registrar.MapProfiles.Contexts
{
    public class AdvertFavoriteProfile : Profile
    {
        public AdvertFavoriteProfile()
        {
            CreateMap<AdvertFavorite, AdvertFavoriteSummary>()
                .ForMember(af => af.Id, map => map.MapFrom(a => a.Advert.Id))
                .ForMember(af => af.Address, map => map.MapFrom(a => a.Advert.Address))
                .ForMember(af => af.Price, map => map.MapFrom(a => a.Advert.Price))
                .ForMember(af => af.CreatedAt, map => map.MapFrom(a => a.Advert.CreatedAt))
                .ForMember(af => af.Name, map => map.MapFrom(a => a.Advert.Name));
        }
    }
}
