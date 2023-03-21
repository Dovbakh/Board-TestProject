using Board.Contracts.Contexts.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Comments.Services
{
    /// <summary>
    /// Сервис для работы с категориями.
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// Получить все категории с пагинацией.
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="CommentDetails"/>.</returns>
        Task<IReadOnlyCollection<CommentDetails>> GetAllAsync(int? offset, int? count, CancellationToken cancellation);

        /// <summary>
        /// Получить все категории с пагинацией.
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="CommentDetails"/>.</returns>
        Task<IReadOnlyCollection<CommentDetails>> GetAllFilteredAsync(CommentFilterRequest filterRequest, int? offset, int? count, CancellationToken cancellation);

        /// <summary>
        /// Получить категорию по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор категории.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Элемент <see cref="CommentDetails"/>.</returns>
        Task<CommentDetails> GetByIdAsync(int id, CancellationToken cancellation);

        /// <summary>
        /// Добавить новый комментарий.
        /// </summary>
        /// <param name="addRequest">Элемент <see cref="CommentAddRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор новой категории.</returns>
        Task<int> AddAsync(CommentAddRequest addRequest, CancellationToken cancellation);

        /// <summary>
        /// Изменить категорию.
        /// </summary>
        /// <param name="id">Идентификатор категории.</param>
        /// <param name="updateRequest">Элемент <see cref="CommentUpdateRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task UpdateAsync(int id, CommentUpdateRequest updateRequest, CancellationToken cancellation);

        /// <summary>
        /// Удалить категорию.
        /// </summary>
        /// <param name="id">Идентификатор категории.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(int id, CancellationToken cancellation);
    }
}
