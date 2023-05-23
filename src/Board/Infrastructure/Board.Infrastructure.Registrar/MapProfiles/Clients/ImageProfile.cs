using AutoMapper;
using Board.Contracts.Contexts.Images;
using FileStorage.Contracts.Clients.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Registrar.MapProfiles.Clients
{
    /// <summary>
    /// Профиль AutoMapper для работы с Image
    /// </summary>
    public class ImageProfile : Profile
    {
        public ImageProfile() 
        {
            CreateMap<ImageShortInfoClientResponse, ImageShortInfo>();

            CreateMap<ImageDataClientResponse, ImageData>();
        }
    }
}
