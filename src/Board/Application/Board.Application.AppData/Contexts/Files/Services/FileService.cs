using AutoMapper;
using Board.Contracts.Contexts.Files;
using FileStorage.Clients.Contexts.Files;
using FileStorage.Contracts.Clients.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Files.Services
{
    /// <inheritdoc />
    public class FileService : IFileService
    {
        private readonly IMapper _mapper;
        private readonly IFileClient _fileClient;
        private readonly ILogger<FileService> _logger;
        public FileService(IMapper mapper, IFileClient fileClient, ILogger<FileService> logger)
        {
            _mapper = mapper;
            _fileClient = fileClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<FileData> DownloadAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Скачивание файла с ID: {1}",
                nameof(DownloadAsync), id);

            var clientResponse = await _fileClient.DownloadAsync(id, cancellation);
            var fileData = _mapper.Map<FileDataClientResponse, FileData>(clientResponse);

            return fileData;
        }

        /// <inheritdoc />
        public async Task<FileShortInfo> GetInfoAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение информации о файле с ID: {1}",
                nameof(GetInfoAsync), id);

            var clientResponse = await _fileClient.GetInfoAsync(id, cancellation);
            var fileInfo = _mapper.Map<FileShortInfoClientResponse, FileShortInfo>(clientResponse);

            return fileInfo;
        }

        /// <inheritdoc />
        public async Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Загрузка файла с содержимым: {1}",
                nameof(UploadAsync), JsonConvert.SerializeObject(file));

            if (file.ContentType != "image/png" && file.ContentType != "image/jpeg")
            {
                throw new InvalidOperationException("Неподдерживаемый формат изображения.");
            }

            var fileId = await _fileClient.UploadAsync(file, cancellation);

            return fileId;
        }

        /// <inheritdoc />
        public Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление файла с ID: {1}",
                nameof(DeleteAsync), id);

            return _fileClient.DeleteAsync(id, cancellation);
        }
    }
}
