using Board.Contracts.Contexts.Categories;
using Board.Contracts.Contexts.Comments;
using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Categories.Repositories
{
    /// <summary>
    /// Репозиторий чтения/записи для работы с категориями.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Получить все категории с пагинацией.
        /// </summary>
        /// <param name="take">Количество получаемых категорий.</param>
        /// <param name="skip">Количество пропускаемых категорий.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="CategoryDetails"/>.</returns>
        Task<IReadOnlyCollection<CategorySummary>> GetAllAsync(CancellationToken cancellation);

        Task<IReadOnlyCollection<CategorySummary>> GetAllFilteredAsync(CategoryFilterRequest filterRequest, CancellationToken cancellation);

        /// <summary>
        /// Получить категорию по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор категории.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Элемент <see cref="CategoryDetails"/>.</returns>
        Task<CategoryDetails> GetByIdAsync(Guid id, CancellationToken cancellation);

        /// <summary>
        /// Добавить новую категорию.
        /// </summary>
        /// <param name="categoryDto">Элемент <see cref="CategoryDetails"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор новой категории.</returns>
        Task<Guid> AddAsync(CategoryCreateRequest createRequest, CancellationToken cancellation);


        /// <summary>
        /// Изменить категорию.
        /// </summary>
        /// <param name="id">Идентификатор категории.</param>
        /// <param name="categoryDto">Элемент <see cref="CategoryDetails"/>.</param>
        Task<CategoryDetails> UpdateAsync(Guid id, CategoryUpdateRequest updateRequest, CancellationToken cancellation);

        /// <summary>
        /// Удалить категорию.
        /// </summary>
        /// <param name="id">Идентификатор категории.</param>
        Task DeleteAsync(Guid id, CancellationToken cancellation);
    }
}
