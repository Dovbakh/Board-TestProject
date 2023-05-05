using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Repository
{
    /// <inheritdoc />
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext DbContext { get; }
        protected DbSet<TEntity> DbSet { get; }

        private readonly ILogger<Repository<TEntity>> _logger;

        /// <summary>
        /// Инициализировать экземпляр <see cref="Repository"/>.
        /// </summary>
        /// <param name="context">Контекст БД.</param>
        public Repository(DbContext context, ILogger<Repository<TEntity>> logger)
        {
            DbContext = context;
            DbSet = DbContext.Set<TEntity>();
            _logger = logger;
        }

        /// <inheritdoc />
        public IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        /// <inheritdoc />
        public IQueryable<TEntity> GetAllFiltered(Expression<Func<TEntity, bool>> predicat)
        {
            _logger.LogInformation("{0} -> Создание запроса в базу данных на получение списка сущностей {1} по выражению: {2}",
                nameof(GetAllFiltered), nameof(TEntity), JsonConvert.SerializeObject(predicat));

            if (predicat == null)
            {
                throw new ArgumentNullException($"Выражение для запроса в базу данных не должно быть пустым.");
            }
            return DbSet.Where(predicat);
        }

        /// <inheritdoc />
        public async Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение сущности {1} c ID: {2} из базы данных.",
                nameof(GetByIdAsync), nameof(TEntity), id);

            return await DbSet.FindAsync(id, cancellation);
        }

        /// <inheritdoc />
        public async Task AddAsync(TEntity model, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Добавление сущности {1} c моделью: {2} из базы данных.",
                nameof(AddAsync), nameof(TEntity), JsonConvert.SerializeObject(model));

            if (model == null)
            {
                throw new ArgumentNullException($"Добавляемая сущность не должна быть пустой.");
            }

            try
            {
                await DbSet.AddAsync(model, cancellation);
                await DbContext.SaveChangesAsync(cancellation);
            }
            catch (Exception ex) 
            {
                throw new DbUpdateException($"Не удалось добавить сущность в базу данных.");
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(TEntity model, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Обновление сущности {1} c моделью: {2} в базе данных.",
                nameof(UpdateAsync), nameof(TEntity), JsonConvert.SerializeObject(model));

            if (model == null)
            {
                throw new ArgumentNullException($"Обновляемая сущность не должна быть пустой.");
            }

            try
            {
                DbSet.Update(model);
                await DbContext.SaveChangesAsync(cancellation);
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Не удалось обновить сущность в базе данных.");
            }        
        }

        /// <inheritdoc />
        public async Task DeleteAsync(TEntity model, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление сущности {1} c моделью: {2} в базе данных.",
                nameof(DeleteAsync), nameof(TEntity), JsonConvert.SerializeObject(model));

            if (model == null)
            {
                throw new ArgumentNullException($"Удаляемая сущность не должна быть пустой.");
            }

            
            try
            {
                DbSet.Remove(model);
                await DbContext.SaveChangesAsync(cancellation);
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Не удалось удалить сущность в базе данных.");
            }
        }



    }
}
