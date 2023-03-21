using Board.Contracts.Contexts.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Posts.Repositories
{
    /// <summary>
    /// Репозиторий чтения/записи для работы с обьявлениями.
    /// </summary>
    public interface IAdvertRepository
    {
        /// <summary>
        /// Получить все обьявления с пагинацией.
        /// </summary>
        /// <param name="take">Количество получаемых обьявлений.</param>
        /// <param name="skip">Количество пропускаемых обьявлений.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="AdvertSummary"/>.</returns>
        Task<IReadOnlyCollection<AdvertSummary>> GetAllAsync(int take, int skip, CancellationToken cancellation);

        /// <summary>
        /// Получить все обьявления по фильтру и с пагинацией.
        /// </summary>
        /// <param name="filter">Фильтр <see cref="AdvertFilterRequest"/> для поиска.</param>
        /// <param name="take">Количество получаемых обьявлений.</param>
        /// <param name="skip">Количество пропускаемых обьявлений.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns></returns>
        Task<IReadOnlyCollection<AdvertSummary>> GetAllFilteredAsync(AdvertFilterRequest filter, int take, int skip, CancellationToken cancellation);

        /// <summary>
        /// Получить обьявление по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Элемент <see cref="AdvertDetails"/>.</returns>
        Task<AdvertDetails> GetByIdAsync(int id, CancellationToken cancellation);

        /// <summary>
        /// Добавить новое обьявление.
        /// </summary>
        /// <param name="addRequest">Элемент <see cref="AdvertAddRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор нового обьявления.</returns>
        Task<int> AddAsync(AdvertAddRequest addRequest, CancellationToken cancellation);

        /// <summary>
        /// Изменить обьявление.
        /// </summary>
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="updateRequest">Элемент <see cref="AdvertUpdateRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task UpdateAsync(int id, AdvertUpdateRequest updateRequest, CancellationToken cancellation);

        /// <summary>
        /// Удалить обьявление.
        /// </summary>
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(int id, CancellationToken cancellation);

    }
}
