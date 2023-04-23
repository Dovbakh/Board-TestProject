using AutoMapper;
using FileStorage.Contracts.Contexts.Files;
using FileStorage.Contracts.Clients.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Infrastructure.Registrar.MapProfiles.Clients.Files
{
    public class FileProfile : Profile
    {
        public FileProfile() 
        {
            CreateMap<FileShortInfo, FileShortInfoClientResponse>();
            CreateMap<FileData, FileDataClientResponse>();
        }
    }
}
