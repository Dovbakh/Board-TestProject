using Board.Contracts.Contexts.Comments;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Comments.Services
{
    /// <summary>
    /// Сервис для работы с отзывами.
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// Получить все отзывы с пагинацией.
        /// </summary>
        /// <param name="offset">Количество пропускаемых отзывов.</param>
        /// <param name="count">Количество получаемых отзывов.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список отзывов.</returns>
        Task<IReadOnlyCollection<CommentDetails>> GetAllAsync(int? offset, int? count, CancellationToken cancellation);

        /// <summary>
        /// Получить все отзывы с пагинацией и фильтрацией.
        /// </summary>
        /// <param name="filterRequest">Модель фильтрации отзывов.</param>
        /// <param name="offset">Количество пропускаемых отзывов.</param>
        /// <param name="count">Количество получаемых отзывов.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список отзывов.</returns>
        Task<IReadOnlyCollection<CommentDetails>> GetAllFilteredAsync(CommentFilterRequest filterRequest, int? offset, int? count, CancellationToken cancellation);


        /// <summary>
        /// Получить отзыв по идентификатору.
        /// </summary>
        /// <param name="commentId">Идентификатор отзыва.</param>
        /// <param name="cancellation"></param>
        /// <returns>Отзыв.</returns>
        Task<CommentDetails> GetByIdAsync(Guid commentId, CancellationToken cancellation);

        /// <summary>
        /// Добавить новый отзыв.
        /// </summary>
        /// <param name="addRequest">Модель добавления отзыва.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор нового отзыва.</returns>
        Task<Guid> CreateAsync(CommentAddRequest addRequest, CancellationToken cancellation);

        /// <summary>
        /// Изменить отзыв.
        /// </summary>
        /// <param name="commentId">Идентификатор отзыва.</param>
        /// <param name="updateRequest">Модель изменения отзыва.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task<CommentDetails> UpdateAsync(Guid commentId, CommentUpdateRequest updateRequest, CancellationToken cancellation);

        /// <summary>
        /// Удалить отзыв, сделав его неактивным.
        /// </summary>
        /// <param name="commentId">Идентификатор отзыва.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task SoftDeleteAsync(Guid commentId, CancellationToken cancellation);

        /// <summary>
        /// Удалить отзыв.
        /// </summary>
        /// <param name="commentId">Идентификатор отзыва.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(Guid commentId, CancellationToken cancellation);
    }
}
