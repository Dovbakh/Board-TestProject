using FileStorage.Application.AppData.Contexts.Files.Repositories;
using FileStorage.Contracts.Contexts.Files;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Application.AppData.Contexts.Files.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            return _fileRepository.DeleteAsync(id, cancellation);
        }

        public Task<FileData> DownloadAsync(Guid id, CancellationToken cancellation)
        {
            return _fileRepository.DownloadAsync(id, cancellation);
        }

        public Task<FileShortInfo> GetInfoAsync(Guid id, CancellationToken cancellation)
        {
            return _fileRepository.GetInfoAsync(id, cancellation);
        }

        public async Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellation)
        {
            if (file.ContentType != "image/png" && file.ContentType != "image/jpeg")
            {
                throw new InvalidOperationException("Неподдерживаемый формат изображения.");
            }

            var bytes = new byte[file.Length];
            await file.OpenReadStream().ReadAsync(bytes, 0, bytes.Length);
            var resizedBytes = await ResizeImage(bytes, 1280, 960, ResizeMode.Max);        
            var fileName = await _fileRepository.UploadAsync(file.ContentType, resizedBytes, cancellation);

            return fileName;
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
            var imageInfo = Image.Identify(imageBytes);
            if (imageInfo == null)
            {
                throw new Exception("Файл не является изображением.");
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
