using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Contexts.Comments;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Adverts.Services
{
    /// <summary>
    /// Сервис для работы с обьявлениями.
    /// </summary>
    public interface IAdvertService
    {
        /// <summary>
        /// Получить все обьявления с пагинацией.
        /// </summary>
        /// <param name="offset">Количество пропускаемых обьявлений.</param>
        /// <param name="limit">Количество получаемых обьявлений.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список обьявлений с краткой информацией.</returns>
        Task<IReadOnlyCollection<AdvertSummary>> GetAllAsync(int? offset, int? limit, CancellationToken cancellation);

        /// <summary>
        /// Получить все обьявления с пагинацией и фильтрацией.
        /// </summary>
        /// <param name="request">Модель фильтрации обьявлений.</param>
        /// <param name="offset">Количество пропускаемых обьявлений.</param>
        /// <param name="limit">Количество получаемых обьявлений.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список обьявлений с краткой информацией.</returns>
        Task<IReadOnlyCollection<AdvertSummary>> GetAllFilteredAsync(AdvertFilterRequest request, int? offset, int? limit, CancellationToken cancellation);

        /// <summary>
        /// Получить обьявление по идентификатору.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Обьявление с детальной информацией.</returns>
        Task<AdvertDetails> GetByIdAsync(Guid advertId, CancellationToken cancellation);

        /// <summary>
        /// Получить отзывы, оставленные к обьявлению с пагинацией.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="offset">Количество пропускаемых отзывов.</param>
        /// <param name="limit">Количество получаемых отзывов.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список отзывов.</returns>
        Task<IReadOnlyCollection<CommentDetails>> GetCommentsByAdvertIdAsync(Guid advertId, int? offset, int? limit, CancellationToken cancellation);

        /// <summary>
        /// Получить отзывы, оставленные к обьявлению с пагинацией и фильтрацией.
        /// </summary>
        /// <param name="filterRequest">Модель фильтрации отзывов.</param>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="offset">Количество пропускаемых отзывов.</param>
        /// <param name="limit">Количество получаемых отзывов.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список отзывов.</returns>
        Task<IReadOnlyCollection<CommentDetails>> GetFilteredCommentsByAdvertIdAsync(Guid advertId, CommentFilterRequest filterRequest, int? offset, int? limit,
            CancellationToken cancellation);

        /// <summary>
        /// Добавить новое обьявление.
        /// </summary>
        /// <param name="addRequest">Модель добавления нового обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор нового обьявления.</returns>
        Task<Guid> CreateAsync(AdvertAddRequest addRequest, CancellationToken cancellation);

        /// <summary>
        /// Изменить обьявление.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="updateRequest">Модель изменения обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Измененное обьявление с детальной информацией.</returns>
        Task<AdvertDetails> UpdateAsync(Guid advertId, AdvertUpdateRequest updateRequest, CancellationToken cancellation);

        /// <summary>
        /// Удалить обьявление.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(Guid advertId, CancellationToken cancellation);

        /// <summary>
        /// Удалить обьявление, сделав его неактивным.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task SoftDeleteAsync(Guid advertId, CancellationToken cancellation);
    }

}
