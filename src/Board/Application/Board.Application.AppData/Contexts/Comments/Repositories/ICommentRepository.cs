﻿using Board.Contracts.Contexts.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Comments.Repositories
{
    /// <summary>
    /// Репозиторий чтения/записи для работы с категориями.
    /// </summary>
    public interface ICommentRepository
    {
        /// <summary>
        /// Получить комментарий по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор комментария.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Элемент <see cref="CommentDetails"/>.</returns>
        Task<CommentDetails> GetByIdAsync(int id, CancellationToken cancellation);

        /// <summary>
        /// Получить все комментарии по фильтру с пагинацией.
        /// </summary>
        /// <param name="filterRequest">Фильтр для поиска.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Элемент <see cref="CommentDetails"/>.</returns>
        Task<IReadOnlyCollection<CommentDetails>> GetAllFilteredAsync(CommentFilterRequest filterRequest, int take, int skip, CancellationToken cancellation);


        /// <summary>
        /// Добавить новую комментарий.
        /// </summary>
        /// <param name="addRequest">Элемент <see cref="CommentAddRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор новой комментария.</returns>
        Task<int> AddAsync(CommentAddRequest addRequest, CancellationToken cancellation);

        /// <summary>
        /// Изменить комментарий.
        /// </summary>
        /// <param name="id">Идентификатор комментария.</param>
        /// <param name="updateRequest">Элемент <see cref="CommentUpdateRequest"/>.</param>
        Task UpdateAsync(int id, CommentUpdateRequest updateRequest, CancellationToken cancellation);

        /// <summary>
        /// Удалить комментарий.
        /// </summary>
        /// <param name="id">Идентификатор комментария.</param>
        Task DeleteAsync(int id, CancellationToken cancellation);
    }
}