using Board.Contracts.Contexts.AdvertImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertImages.Repositories
{
    /// <summary>
    /// Репозиторий для чтения\записи изображений из обьявления.
    /// </summary>
    public interface IAdvertImageRepository
    {
        /// <summary>
        /// Получить все изображения по идентификатору обьявления.
        /// </summary>
        /// <param name="postId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="AdvertImageDto"/>.</returns>
        Task<IReadOnlyCollection<AdvertImageDto>> GetAllByPostIdAsync(int postId, CancellationToken cancellation);


        /// <summary>
        /// Получить изображение по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Элемент <see cref="AdvertImageDto"/>.</returns>
        Task<AdvertImageDto> GetByIdAsync(int id, CancellationToken cancellation);


        /// <summary>
        /// Добавить изображение.
        /// </summary>
        /// <param name="addRequest">Элемент <see cref="AdvertImageAddRequest"/></param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns></returns>
        Task<int> AddAsync(AdvertImageAddRequest addRequest, CancellationToken cancellation);


        /// <summary>
        /// Удалить изображение.
        /// </summary>
        /// <param name="id">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(int id, CancellationToken cancellation);


    }
}
