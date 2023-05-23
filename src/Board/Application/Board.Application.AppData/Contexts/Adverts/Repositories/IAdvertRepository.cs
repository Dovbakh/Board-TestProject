using Board.Contracts.Contexts.Adverts;
using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Adverts.Repositories
{
    /// <summary>
    /// Репозиторий для работы с обьявлениями.
    /// </summary>
    public interface IAdvertRepository
    {
        /// <summary>
        /// Получить все обьявления с пагинацией.
        /// </summary>
        /// <param name="offset">Количество пропускаемых обьявлений.</param>
        /// <param name="limit">Количество получаемых обьявлений.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список обьявлений с краткой информацией.</returns>
        Task<IReadOnlyCollection<AdvertSummary>> GetAllAsync(int offset, int limit, CancellationToken cancellation);

        /// <summary>
        /// Получить все обьявления с пагинацией и фильтрацией.
        /// </summary>
        /// <param name="request">Модель фильтрации обьявлений.</param>
        /// <param name="offset">Количество пропускаемых обьявлений.</param>
        /// <param name="limit">Количество получаемых обьявлений.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список обьявлений с краткой информацией.</returns>
        Task<IReadOnlyCollection<AdvertSummary>> GetAllFilteredAsync(AdvertFilterRequest filter, int offset, int count, CancellationToken cancellation);

        /// <summary>
        /// Получить все обьявления с идентификаторами из списка с пагинацией.
        /// </summary>
        /// <param name="advertIds">Список идентификаторов обьявлений.</param>
        /// <param name="offset">Количество пропускаемых обьявлений.</param>
        /// <param name="limit">Количество получаемых обьявлений.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список обьявлений с краткой информацией.</returns>
        Task<IReadOnlyCollection<AdvertSummary>> GetByListIdAsync(List<Guid> advertIds, int offset, int limit, CancellationToken cancellation);

        /// <summary>
        /// Получить все избранные обьявления пользователя с пагинацией.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="offset">Количество пропускаемых обьявлений.</param>
        /// <param name="limit">Количество получаемых обьявлений.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список обьявлений с краткой информацией.</returns>
        Task<IReadOnlyCollection<AdvertSummary>> GetFavoritesByUserIdAsync(Guid userId, int offset, int limit, CancellationToken cancellation);

        /// <summary>
        /// Получить обьявление по идентификатору.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Обьявление с детальной информацией.</returns>
        Task<AdvertDetails> GetByIdAsync(Guid advertId, CancellationToken cancellation);

        /// <summary>
        /// Получить идентификатор пользователя, создавшего обьявление.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор пользователя.</returns>
        Task<Guid> GetUserIdAsync(Guid advertId, CancellationToken cancellation);

        /// <summary>
        /// Проверить наличие обьявления.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Индикатор наличия обьявления.</returns>
        Task<bool> IsExists(Guid advertId, CancellationToken cancellation);

        /// <summary>
        /// Добавить новое обьявление.
        /// </summary>
        /// <param name="addRequest">Модель добавления нового обьявления..</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор нового обьявления.</returns>
        Task<Guid> AddAsync(AdvertAddRequest addRequest, CancellationToken cancellation);

        /// <summary>
        /// Изменить обьявление.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="updateRequest">Модель изменения обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task<AdvertDetails> UpdateAsync(Guid advertId, AdvertUpdateRequest updateRequest, CancellationToken cancellation);

        /// <summary>
        /// Удалить обьявление, сделав неактивным.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task SoftDeleteAsync(Guid advertId, CancellationToken cancellation);

        /// <summary>
        /// Удалить обьявление.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(Guid advertId, CancellationToken cancellation);

    }
}
