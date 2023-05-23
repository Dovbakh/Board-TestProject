using FileStorage.Contracts.Contexts.Images;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Application.AppData.Contexts.Images.Services
{
    public interface IImageService
    {
        /// <summary>
        /// Получить информацию об изображении.
        /// </summary>
        /// <param name="imageId">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Краткая информация об изображении.</returns>
        public Task<ImageShortInfo> GetInfoAsync(Guid imageId, CancellationToken cancellation);

        /// <summary>
        /// Загрузить изображение на сервер.
        /// </summary>
        /// <param name="image">Файл с изображением.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор загруженного изображения.</returns>
        public Task<Guid> UploadAsync(IFormFile image, CancellationToken cancellation);

        /// <summary>
        /// Скачать изображение с сервера.
        /// </summary>
        /// <param name="id">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Модель содержащая байты и тип изображения.</returns>
        public Task<ImageData> DownloadAsync(Guid imageId, CancellationToken cancellation);

        /// <summary>
        /// Удалить изображение.
        /// </summary>
        /// <param name="id">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        public Task DeleteAsync(Guid imageId, CancellationToken cancellation);

        /// <summary>
        /// Проверка на наличие изображения на сервере.
        /// </summary>
        /// <param name="id">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>true - изображение есть, false - изображение не найдено.</returns>
        public Task<bool> IsImageExists(Guid imageId, CancellationToken cancellation);

    }
}
