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
    public class ImageProfile : Profile
    {
        public ImageProfile() 
        {
            CreateMap<ImageShortInfoClientResponse, ImageShortInfo>();
            CreateMap<ImageDataClientResponse, ImageData>();
        }
    }
}
