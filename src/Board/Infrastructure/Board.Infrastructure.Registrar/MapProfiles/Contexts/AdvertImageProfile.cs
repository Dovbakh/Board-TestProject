﻿using AutoMapper;
using Board.Contracts.Contexts.AdvertImages;
using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Registrar.MapProfiles.Contexts
{
    /// <summary>
    /// Профиль AutoMapper для работы с AdvertImage
    /// </summary>
    public class AdvertImageProfile : Profile
    {
        public AdvertImageProfile()
        {
            CreateMap<AdvertImageAddRequest, AdvertImage>()
                .ForMember(a => a.Id, map => map.Ignore())
                .ForMember(a => a.IsActive, map => map.MapFrom(s => true))
                .ForMember(a => a.CreatedAt, map => map.MapFrom(s => DateTime.UtcNow))
                .ForMember(a => a.Advert, map => map.Ignore());

            CreateMap<AdvertImage, AdvertImageDto>();
        }
    }
}
