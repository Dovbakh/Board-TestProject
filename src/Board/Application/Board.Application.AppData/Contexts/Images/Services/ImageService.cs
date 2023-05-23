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
        private readonly IImageClient _imageClient;
        private readonly ILogger<ImageService> _logger;
        private readonly IValidator<IFormFile> _imageUploadValidator;
        public ImageService(IMapper mapper, IImageClient imageClient, ILogger<ImageService> logger, IValidator<IFormFile> imageUploadValidator)
        {
            _mapper = mapper;
            _imageClient = imageClient;
            _logger = logger;
            _imageUploadValidator = imageUploadValidator;
        }

        /// <inheritdoc />
        public async Task<ImageData> DownloadAsync(Guid imageId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Скачивание файла с ID: {1}",
                nameof(DownloadAsync), imageId);

            var clientResponse = await _imageClient.DownloadAsync(imageId, cancellation);
            var imageData = _mapper.Map<ImageDataClientResponse, ImageData>(clientResponse);

            return imageData;
        }

        /// <inheritdoc />
        public async Task<ImageShortInfo> GetInfoAsync(Guid imageId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение информации о файле с ID: {1}",
                nameof(GetInfoAsync), imageId);

            var clientResponse = await _imageClient.GetInfoAsync(imageId, cancellation);
            var imageInfo = _mapper.Map<ImageShortInfoClientResponse, ImageShortInfo>(clientResponse);

            return imageInfo;
        }

        /// <inheritdoc />
        public async Task<bool> IsImageExists(Guid imageId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение информации о файле с ID: {1}",
                nameof(GetInfoAsync), imageId);

            var isImageExists = await _imageClient.IsImageExists(imageId, cancellation);

            return isImageExists;
        }

        /// <inheritdoc />
        public async Task<Guid> UploadAsync(IFormFile image, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Загрузка файла с содержимым: {1}",
                nameof(UploadAsync), JsonConvert.SerializeObject(image));

            await _imageUploadValidator.ValidateAndThrowAsync(image, cancellation);


            var newImageId = await _imageClient.UploadAsync(image, cancellation);

            return newImageId;
        }

        /// <inheritdoc />
        public Task DeleteAsync(Guid imageId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление файла с ID: {1}",
                nameof(DeleteAsync), imageId);

            return _imageClient.DeleteAsync(imageId, cancellation);
        }
    }
}
