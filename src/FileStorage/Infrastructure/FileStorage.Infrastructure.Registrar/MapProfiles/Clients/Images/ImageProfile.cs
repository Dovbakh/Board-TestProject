using AutoMapper;
using FileStorage.Contracts.Contexts.Images;
using FileStorage.Contracts.Clients.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Infrastructure.Registrar.MapProfiles.Clients.Images
{
    public class ImageProfile : Profile
    {
        public ImageProfile() 
        {
            CreateMap<ImageShortInfo, ImageShortInfoClientResponse>();
            CreateMap<ImageData, ImageDataClientResponse>();
        }
    }
}
