using Board.Contracts.Contexts.AdvertFavorites;
using Board.Contracts.Contexts.Adverts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertFavorites.Services
{
    /// <summary>
    /// Сервис для работы с избранными обьявлениями.
    /// </summary>
    public interface IAdvertFavoriteService
    {
        /// <summary>
        /// Получить все избранные обьявления для текущего пользователя (авторизованного или нет) с пагинацией.
        /// </summary>
        /// <param name="offset">Количество пропускаемых обьявлений.</param>
        /// <param name="limit">Количество получаемых обьявлений.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список обьявлений.</returns>
        Task<IReadOnlyCollection<AdvertSummary>> GetAdvertsForCurrentUserAsync(int? offset, int? limit, CancellationToken cancellation);

        /// <summary>
        /// Добавить новое обьявление в избранные, если еще не находится там.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task AddIfNotExistsAsync(Guid advertId, CancellationToken cancellation);

        /// <summary>
        /// Удалить обьявление из избранного.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(Guid advertId, CancellationToken cancellation);

        /// <summary>
        /// Получить все идентификаторы обьявлений в избранном текущего пользователя.
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список идентификаторов.</returns>
        Task<IReadOnlyCollection<Guid>> GetIdsForCurrentUserAsync(CancellationToken cancellation);

        
    }
}
