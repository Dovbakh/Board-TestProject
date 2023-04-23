using AutoMapper;
using Board.Contracts.Contexts.Files;
using FileStorage.Contracts.Clients.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Registrar.MapProfiles.Clients
{
    public class FileProfile : Profile
    {
        public FileProfile() 
        {
            CreateMap<FileShortInfoClientResponse, FileShortInfo>();
            CreateMap<FileDataClientResponse, FileData>();
        }
    }
}
