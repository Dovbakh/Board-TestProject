using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Repository
{
    /// <summary>
    /// Базовый репозиторий.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Получить все элементы сущности <see cref="TEntity"/>.
        /// </summary>
        /// <returns>Все элементы сущности <see cref="TEntity"/>.</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Получить все элементы сущности <see cref="TEntity"/> по фильтру.
        /// </summary>
        /// <param name="filter">Фильтр.</param>
        /// <returns>Все элементы сущности <see cref="TEntity"/> по фильтру.</returns>
        IQueryable<TEntity> GetAllFiltered(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Получить элемент <see cref="TEntity"/> по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор <see cref="TEntity"/>.</param>
        /// <returns><see cref="TEntity"/>.</returns>
        Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellation);

        /// <summary>
        /// Добавить элемент <see cref="TEntity"/>.
        /// </summary>
        /// <param name="model">Новая сущность <see cref="TEntity"/>.</param>
        Task AddAsync(TEntity model, CancellationToken cancellation);

        /// <summary>
        /// Изменить элемент <see cref="TEntity"/>.
        /// </summary>
        /// <param name="model">Существующая сущность <see cref="TEntity"/>.</param>
        Task UpdateAsync(TEntity model, CancellationToken cancellation);

        /// <summary>
        /// Удалить элемент <see cref="TEntity"/>.
        /// </summary>
        /// <param name="model">Существующая сущность <see cref="TEntity"/>.</param>
        Task DeleteAsync(TEntity model, CancellationToken cancellation);
    }
}
