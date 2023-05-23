using Board.Contracts.Contexts.Images;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Images.Services
{
    /// <summary>
    /// Сервис для работы с изображениями.
    /// </summary>
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
        public Task<ImageData> DownloadAsync(Guid id, CancellationToken cancellation);

        /// <summary>
        /// Удалить изображение.
        /// </summary>
        /// <param name="id">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        public Task DeleteAsync(Guid id, CancellationToken cancellation);

        /// <summary>
        /// Проверка на наличие изображения на сервере.
        /// </summary>
        /// <param name="id">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>true - изображение есть, false - изображение не найдено.</returns>
        Task<bool> IsImageExists(Guid id, CancellationToken cancellation);
    }
}
