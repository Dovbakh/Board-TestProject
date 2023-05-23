using Board.Contracts.Contexts.AdvertImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertImages.Repositories
{
    /// <summary>
    /// Репозиторий для работы изображений из обьявления.
    /// </summary>
    public interface IAdvertImageRepository
    {
        /// <summary>
        /// Получить все изображения по идентификатору обьявления.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="AdvertImageDto"/>.</returns>
        Task<IReadOnlyCollection<AdvertImageDto>> GetAllByAdvertIdAsync(Guid advertId, CancellationToken cancellation);


        /// <summary>
        /// Получить изображение по идентификатору.
        /// </summary>
        /// <param name="imageId">Идентификатор.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Элемент <see cref="AdvertImageDto"/>.</returns>
        Task<AdvertImageDto> GetByIdAsync(Guid imageId, CancellationToken cancellation);


        /// <summary>
        /// Добавить изображение.
        /// </summary>
        /// <param name="addRequest">Элемент <see cref="AdvertImageAddRequest"/></param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор записи.</returns>
        Task<Guid> AddAsync(AdvertImageAddRequest addRequest, CancellationToken cancellation);

        /// <summary>
        /// Проверка наличия записи об отношении изображения к обьявлению.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="imageId">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Индикатор наличия записи об отношении изображения к обьявлению</returns>
        Task<bool> IsExists(Guid advertId, Guid imageId, CancellationToken cancellation);

        /// <summary>
        /// Удалить запись.
        /// </summary>
        /// <param name="advertImageId">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(Guid advertImageId, CancellationToken cancellation);

        /// <summary>
        /// Удалить запись по идентификатору изображения.
        /// </summary>
        /// <param name="imageId">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteByFileIdAsync(Guid imageId, CancellationToken cancellation);
    }
}
