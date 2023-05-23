using FileStorage.Application.AppData.Contexts.Images.Repositories;
using FileStorage.Contracts.Contexts.Images;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Application.AppData.Contexts.Images.Services
{
    /// <inheritdoc />
    public class ImageService : IImageService
    {
        private readonly IImageRepository _fileRepository;
        private readonly ILogger<ImageService> _logger;
        private readonly IValidator<IFormFile> _imageUploadValidator;

        public ImageService(IImageRepository fileRepository, ILogger<ImageService> logger, IValidator<IFormFile> imageUploadValidator)
        {
            _fileRepository = fileRepository;
            _logger = logger;
            _imageUploadValidator = imageUploadValidator;
        }

        /// <inheritdoc />
        public Task<ImageData> DownloadAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Скачивание файла с ID: {1}",
                nameof(DownloadAsync), id);

            return _fileRepository.DownloadAsync(id, cancellation);
        }

        /// <inheritdoc />
        public async Task<ImageShortInfo> GetInfoAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение информации о файле с ID: {1}",
                nameof(GetInfoAsync), id);

            var imageInfo = await _fileRepository.GetInfoAsync(id, cancellation);
            if (imageInfo == null)
            {
                throw new KeyNotFoundException($"Не найдено изображение с ID: {id}");
            }

            return imageInfo;
        }

        /// <inheritdoc />
        public async Task<bool> IsImageExists(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение информации о файле с ID: {1}",
                nameof(GetInfoAsync), id);

            var isExists = await _fileRepository.IsExists(id, cancellation);
            if (isExists)
            {
                return true;
            }

            return false;
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

            var bytes = new byte[file.Length];
            await file.OpenReadStream().ReadAsync(bytes, 0, bytes.Length);
            var resizedBytes = await ResizeImage(bytes, 1280, 960, ResizeMode.Max);        
            var fileName = await _fileRepository.UploadAsync(file.ContentType, resizedBytes, cancellation);

            return fileName;
        }

        /// <inheritdoc />
        public Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление файла с ID: {1}",
                nameof(DeleteAsync), id);

            return _fileRepository.DeleteAsync(id, cancellation);
        }

        /// <summary>
        /// Изменить размер изображения.
        /// </summary>
        /// <param name="imageBytes">Массив байтов с содержимым файла-изображения.</param>
        /// <param name="width">Необходимая ширина.</param>
        /// <param name="height">Необходимая высота.</param>
        /// <param name="mode">Режим изменения размера.</param>
        /// <returns>Массив байтов с содержимым измененного файла-изображения.</returns>
        private async Task<byte[]> ResizeImage(byte[] imageBytes, int width, int height, ResizeMode mode)
        {
            _logger.LogInformation("{0} -> Изменение размера изображения на {1}x{2} из байтов: {3}",
                nameof(UploadAsync), width, height, imageBytes);

            var imageInfo = Image.Identify(imageBytes);
            if (imageInfo == null)
            {
                throw new ArgumentException("Файл не является изображением.");
            }

            var image = Image.Load(imageBytes);

            var options = new ResizeOptions() { Mode = mode, Size = new Size(width, height) };
            image.Mutate(x => x.Resize(options));

            using (var stream = new MemoryStream())
            {
                await image.SaveAsJpegAsync(stream);
                stream.Position = 0;
                return stream.ToArray();
            }
        }
    }
}
