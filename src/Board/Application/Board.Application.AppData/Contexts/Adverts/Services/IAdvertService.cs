using Board.Contracts.Contexts.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Posts.Services
{
    /// <summary>
    /// Сервис для работы с обьявлениями.
    /// </summary>
    public interface IAdvertService
    {
        /// <summary>
        /// Получить все обьявления с пагинацией.
        /// </summary>
        /// <param name="page">Номер страницы.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="AdvertSummary"/>.</returns>
        Task<IReadOnlyCollection<AdvertSummary>> GetAllAsync(int? offset, int? count, CancellationToken cancellation);

        /// <summary>
        /// Получить все обьявления по фильтру и с пагинацией.
        /// </summary>
        /// <param name="request">Фильтр <see cref="AdvertFilterRequest"/> для поиска.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <param name="page">Номер страницы.</param>
        /// <returns>Коллекция элементов <see cref="AdvertSummary"/>.</returns>
        Task<IReadOnlyCollection<AdvertSummary>> GetAllFilteredAsync(AdvertFilterRequest request, int? offset, int? count, CancellationToken cancellation);

        /// <summary>
        /// Получить обьявление по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Элемент <see cref="AdvertDetails"/>.</returns>
        Task<AdvertDetails> GetByIdAsync(int id, CancellationToken cancellation);

        ///// <summary>
        ///// Получить историю просмотренных обьявлений текущего пользователя.
        ///// </summary>
        ///// <param name="page">Номер страницы.</param>
        ///// <param name="cancellation">Токен отмены.</param>
        ///// <returns>Коллекция элементов <see cref="AdvertisementResponseDto"/>.</returns>
        //Task<IReadOnlyCollection<AdvertisementResponseDto>> GetHistoryAsync(int? page, CancellationToken cancellation);

        ///// <summary>
        ///// Получить N-количество последних просмотренных обьявлений.
        ///// </summary>
        ///// <param name="count">Количество последних просмотренных обьявлений.</param>
        ///// <param name="cancellation">Токен отмены.</param>
        ///// <returns>Коллекция элементов <see cref="AdvertisementResponseDto"/>.</returns>
        //Task<IReadOnlyCollection<AdvertisementResponseDto>> GetLastViewedAsync(int? count, CancellationToken cancellation);

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
