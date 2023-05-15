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
            CreateMap<AdvertFavorite, AdvertFavoriteSummary>();
        }
    }
}
