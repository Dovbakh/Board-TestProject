using AutoMapper;
using Board.Application.AppData.Contexts.Adverts.Helpers;
using Board.Contracts.Contexts.Images;
using FileStorage.Clients.Contexts.Images;
using FileStorage.Contracts.Clients.Images;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Images.Services
{
    /// <inheritdoc />
    public class ImageService : IImageService
    {
        private readonly IMapper _mapper;
        private readonly IImageClient _fileClient;
        private readonly ILogger<ImageService> _logger;
        private readonly IValidator<IFormFile> _imageUploadValidator;
        public ImageService(IMapper mapper, IImageClient fileClient, ILogger<ImageService> logger, IValidator<IFormFile> imageUploadValidator)
        {
            _mapper = mapper;
            _fileClient = fileClient;
            _logger = logger;
            _imageUploadValidator = imageUploadValidator;
        }

        /// <inheritdoc />
        public async Task<ImageData> DownloadAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Скачивание файла с ID: {1}",
                nameof(DownloadAsync), id);

            var clientResponse = await _fileClient.DownloadAsync(id, cancellation);
            var fileData = _mapper.Map<ImageDataClientResponse, ImageData>(clientResponse);

            return fileData;
        }

        /// <inheritdoc />
        public async Task<ImageShortInfo> GetInfoAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение информации о файле с ID: {1}",
                nameof(GetInfoAsync), id);

            var clientResponse = await _fileClient.GetInfoAsync(id, cancellation);
            var fileInfo = _mapper.Map<ImageShortInfoClientResponse, ImageShortInfo>(clientResponse);

            return fileInfo;
        }

        /// <inheritdoc />
        public async Task<bool> IsImageExists(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение информации о файле с ID: {1}",
                nameof(GetInfoAsync), id);

            var isImageExists = await _fileClient.IsImageExists(id, cancellation);

            return isImageExists;
        }

        /// <inheritdoc />
        public async Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Загрузка файла с содержимым: {1}",
                nameof(UploadAsync), JsonConvert.SerializeObject(file));

            var validationResult = _imageUploadValidator.Validate(file);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Загружаемый файл не прошел валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
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
