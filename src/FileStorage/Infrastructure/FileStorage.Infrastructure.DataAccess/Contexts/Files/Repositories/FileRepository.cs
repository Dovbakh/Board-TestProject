using FileStorage.Application.AppData.Contexts.Files.Repositories;
using FileStorage.Contracts.Contexts.Files;
using FileStorage.Infrastructure.ObjectStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Infrastructure.DataAccess.Contexts.Files.Repositories
{
    /// <inheritdoc />
    public class FileRepository : IFileRepository
    {
        private readonly IObjectStorage _objectStorage;
        private readonly ILogger<FileRepository> _logger;

        public FileRepository(IObjectStorage objectStorage, ILogger<FileRepository> logger)
        {
            _objectStorage = objectStorage;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<FileData> DownloadAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Скачивание файла с ID: {1}",
                nameof(DownloadAsync), id);

            var fileBytes = await _objectStorage.GetData(id.ToString(), "images", cancellation);
            var file = new FileData { Name = "name", Content = fileBytes, ContentType = "image/jpeg" };

            return file;
        }

        /// <inheritdoc />
        public async Task<FileShortInfo> GetInfoAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение информации о файле с ID: {1}",
                nameof(GetInfoAsync), id);

            var objectStat = await _objectStorage.GetInfo(id.ToString(), "images", cancellation);
            var fileInfo = new FileShortInfo { 
                Name = objectStat.ObjectName,
                CreatedAt = objectStat.LastModified, 
                Id = id, 
                Length = objectStat.Size, 
                ContentType = objectStat.ContentType 
            };

            return fileInfo;
        }

        /// <inheritdoc />
        public async Task<Guid> UploadAsync(string contentType, byte[] bytes, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Загрузка файла с содержимым: {1}",
                nameof(UploadAsync), bytes);

            var fileName = Guid.NewGuid();
            var fileFolder = "images";

            await _objectStorage.Upload(fileName.ToString(), fileFolder, contentType, bytes, cancellation);

            return fileName;
        }

        /// <inheritdoc />
        public Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            return _objectStorage.Delete(id.ToString(), "images", cancellation);
        }
    }
}
