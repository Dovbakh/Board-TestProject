using FileStorage.Application.AppData.Contexts.Files.Repositories;
using FileStorage.Contracts.Contexts.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Application.AppData.Contexts.Files.Services
{
    /// <inheritdoc />
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly ILogger<FileService> _logger;

        public FileService(IFileRepository fileRepository, ILogger<FileService> logger)
        {
            _fileRepository = fileRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<FileData> DownloadAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Скачивание файла с ID: {1}",
                nameof(DownloadAsync), id);

            return _fileRepository.DownloadAsync(id, cancellation);
        }

        /// <inheritdoc />
        public Task<FileShortInfo> GetInfoAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение информации о файле с ID: {1}",
                nameof(GetInfoAsync), id);

            return _fileRepository.GetInfoAsync(id, cancellation);
        }

        /// <inheritdoc />
        public async Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Загрузка файла с содержимым: {1}",
                nameof(UploadAsync), JsonConvert.SerializeObject(file));

            if (file.ContentType != "image/png" && file.ContentType != "image/jpeg")
            {
                throw new ArgumentException("Неподдерживаемый формат изображения.");
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
