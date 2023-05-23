using Board.Contracts.Contexts.AdvertFavorites;
using Board.Contracts.Contexts.Adverts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertFavorites.Repositories
{
    /// <summary>
    /// Репозиторий для работы с избранными обьявлениями.
    /// </summary>
    public interface IAdvertFavoriteRepository
    {
        /// <summary>
        /// Получить все идентификаторы обьявлений в избранном указанного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список идентификаторов.</returns>
        Task<IReadOnlyCollection<Guid>> GetIdsByUserIdAsync(Guid userId, CancellationToken cancellation);

        /// <summary>
        /// Добавить новое обьявление в избранные указанного пользователя, если еще не находится там.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task<Guid> AddIfNotExistsAsync(Guid advertId, Guid userId, CancellationToken cancellation);

        /// <summary>
        /// Удалить обьявление из избранного указанного пользователя.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(Guid advertId, Guid userId, CancellationToken cancellation);

        /// <summary>
        /// Добавить новое обьявление в избранные в куки неавторизованного пользователя, если еще не находится там.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        void AddToCookieIfNotExists(Guid advertId, CancellationToken cancellation);

        /// <summary>
        /// Удалить обьявление из избранного в куках неавторизованного пользователя.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        void DeleteFromCookie(Guid advertId, CancellationToken cancellation);

        /// <summary>
        /// Получить все идентификаторы обьявлений в избранном в куках неавторизованного пользователя.
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список идентификаторов.</returns>
        List<Guid> GetIdsFromCookie(CancellationToken cancellation);


    }
}
