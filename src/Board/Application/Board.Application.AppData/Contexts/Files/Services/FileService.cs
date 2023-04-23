using AutoMapper;
using Board.Contracts.Contexts.Files;
using FileStorage.Clients.Contexts.Files;
using FileStorage.Contracts.Clients.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Files.Services
{
    public class FileService : IFileService
    {
        private readonly IMapper _mapper;
        private readonly IFileClient _fileClient;
        public FileService(IMapper mapper, IFileClient fileClient)
        {
            _mapper = mapper;
            _fileClient = fileClient;
        }         

        public async Task<FileData> DownloadAsync(Guid id, CancellationToken cancellation)
        {
            var clientResponse = await _fileClient.DownloadAsync(id, cancellation);
            var fileData = _mapper.Map<FileDataClientResponse, FileData>(clientResponse);

            return fileData;
        }

        public async Task<FileShortInfo> GetInfoAsync(Guid id, CancellationToken cancellation)
        {
            var clientResponse = await _fileClient.GetInfoAsync(id, cancellation);
            var fileInfo = _mapper.Map<FileShortInfoClientResponse, FileShortInfo>(clientResponse);

            return fileInfo;
        }

        public async Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellation)
        {
            var fileId = await _fileClient.UploadAsync(file, cancellation);

            return fileId;
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            return _fileClient.DeleteAsync(id, cancellation);
        }
    }
}
