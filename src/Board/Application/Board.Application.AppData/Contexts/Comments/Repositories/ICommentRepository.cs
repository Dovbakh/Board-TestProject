using Board.Contracts.Contexts.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Comments.Repositories
{
    /// <summary>
    /// Репозиторий для работы с отзывами.
    /// </summary>
    public interface ICommentRepository
    {
        /// <summary>
        /// Получить отзыв по идентификатору.
        /// </summary>
        /// <param name="commentId">Идентификатор отзыва.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Отзыв.</returns>
        Task<CommentDetails> GetByIdAsync(Guid commentId, CancellationToken cancellation);

        /// <summary>
        /// Получить все отзывы c пагинацией.
        /// </summary>
        /// <param name="offset">Количество пропускаемых отзывов.</param>
        /// <param name="limit">Количество получаемых отзывов.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Список отзывов.</returns>
        Task<IReadOnlyCollection<CommentDetails>> GetAllAsync(int offset, int limit, CancellationToken cancellation);

        /// <summary>
        /// Получить все отзывы c пагинацией и фильтрацией.
        /// </summary>
        /// <param name="filterRequest">Модель фильтрации отзывов.</param>
        /// <param name="offset">Количество пропускаемых отзывов.</param>
        /// <param name="limit">Количество получаемых отзывов.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Список отзывов.</returns>
        Task<IReadOnlyCollection<CommentDetails>> GetAllFilteredAsync(CommentFilterRequest filterRequest, int offset, int limit, CancellationToken cancellation);

        /// <summary>
        /// Получить средний рейтинг отзывов, оставленных пользователю.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Средний рейтинг отзывов.</returns>
        Task<float> GetUserRatingAsync(Guid userId, CancellationToken cancellation);

        /// <summary>
        /// Получить идентификатор пользователя, оставившего отзыв.
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="cancellation"></param>
        /// <returns>Идентификатор пользователя.</returns>
        Task<Guid> GetUserIdAsync(Guid commentId, CancellationToken cancellation);

        /// <summary>
        /// Добавить новый отзыв.
        /// </summary>
        /// <param name="addRequest">Модель добавления отзыва.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор нового отзыва.</returns>
        Task<Guid> AddIfNotExistsAsync(CommentAddRequest addRequest, CancellationToken cancellation);

        /// <summary>
        /// Изменить отзыв.
        /// </summary>
        /// <param name="commentId">Идентификатор отзыва.</param>
        /// <param name="updateRequest">Модель изменения отзыва.</param>
        Task<CommentDetails> UpdateAsync(Guid commentId, CommentUpdateRequest updateRequest, CancellationToken cancellation);

        /// <summary>
        /// Удалить отзыв.
        /// </summary>
        /// <param name="commentId">Идентификатор отзыва.</param>
        Task DeleteAsync(Guid commentId, CancellationToken cancellation);

        /// <summary>
        /// Удалить отзыв, сделав его неактивным.
        /// </summary>
        /// <param name="commentId">Идентификатор отзыва.</param>
        Task SoftDeleteAsync(Guid commentId, CancellationToken cancellation);
    }
}
