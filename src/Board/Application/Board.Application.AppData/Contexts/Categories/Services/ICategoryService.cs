using Board.Contracts.Contexts.Categories;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Categories.Services
{
    /// <summary>
    /// Сервис для работы с категориями.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Получить все категории.
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список категорий.</returns>
        Task<IReadOnlyCollection<CategorySummary>> GetAllAsync(CancellationToken cancellation);

        /// <summary>
        /// Получить категорию по идентификатору.
        /// </summary>
        /// <param name="categoryId">Идентификатор категории.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Категория.</returns>
        Task<CategoryDetails> GetByIdAsync(Guid categoryId, CancellationToken cancellation);

        /// <summary>
        /// Добавить новую категорию.
        /// </summary>
        /// <param name="createRequest">Модель добавления категории.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор новой категории.</returns>
        Task<Guid> CreateAsync(CategoryAddRequest createRequest, CancellationToken cancellation);

        /// <summary>
        /// Изменить категорию.
        /// </summary>
        /// <param name="categoryId">Идентификатор категории.</param>
        /// <param name="updateRequest">Модель изменения категории.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Измененная категория.</returns>
        Task<CategoryDetails> UpdateAsync(Guid categoryId, CategoryUpdateRequest updateRequest, CancellationToken cancellation);

        /// <summary>
        /// Удалить категорию.
        /// </summary>
        /// <param name="categoryId">Идентификатор категории.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(Guid categoryId, CancellationToken cancellation);
    }
}
