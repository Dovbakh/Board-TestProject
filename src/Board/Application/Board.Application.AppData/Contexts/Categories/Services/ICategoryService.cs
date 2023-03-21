using Board.Contracts.Contexts.Categories;
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
        /// Получить все категории с пагинацией.
        /// </summary>
        /// <param name="take">Количество получаемых категорий.</param>
        /// <param name="skip">Количество пропускаемых категорий.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="CategoryDetails"/>.</returns>
        Task<IReadOnlyCollection<CategoryDetails>> GetAllAsync(CancellationToken cancellation);

        /// <summary>
        /// Получить категорию по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор категории.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Элемент <see cref="CategoryDetails"/>.</returns>
        Task<CategoryDetails> GetByIdAsync(int id, CancellationToken cancellation);

        /// <summary>
        /// Добавить новую категорию.
        /// </summary>
        /// <param name="categoryDto">Элемент <see cref="CategoryDetails"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор новой категории.</returns>
        Task<int> AddAsync(CategoryDetails categoryDto, CancellationToken cancellation);

        /// <summary>
        /// Изменить категорию.
        /// </summary>
        /// <param name="id">Идентификатор категории.</param>
        /// <param name="categoryDto">Элемент <see cref="CategoryDetails"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task UpdateAsync(int id, CategoryDetails categoryDto, CancellationToken cancellation);

        /// <summary>
        /// Удалить категорию.
        /// </summary>
        /// <param name="id">Идентификатор категории.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(int id, CancellationToken cancellation);
    }
}
