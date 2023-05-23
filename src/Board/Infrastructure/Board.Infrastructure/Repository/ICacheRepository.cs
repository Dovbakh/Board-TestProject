using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Repository
{
    /// <summary>
    /// Репозиторий для работы с распределенным кэшем.
    /// </summary>
    public interface ICacheRepository
    {
        /// <summary>
        /// Получить обьекта из кэша по ключу.
        /// </summary>
        /// <typeparam name="T">Тип обьекта.</typeparam>
        /// <param name="key">Ключ.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Обьект типа <see cref="T"/></returns>
        Task<T> GetById<T>(string key, CancellationToken cancellation) where T : class;

        /// <summary>
        /// Кэшировать обьект со "слайдовым" сроком хранения.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <param name="entity">Обьект.</param>
        /// <param name="slidingTime">Слайдовый срок хранения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task SetWithSlidingTime(string key, object entity, TimeSpan slidingTime, CancellationToken cancellation);

        /// <summary>
        /// Кэшировать обьект со абсолютным сроком хранения.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <param name="entity">Обьект.</param>
        /// <param name="absoluteTime">Абсолютный срок хранения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task SetWithAbsoluteTime(string key, object entity, TimeSpan absoluteTime, CancellationToken cancellation);

        /// <summary>
        /// Удалить обьект из кэша.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(string key, CancellationToken cancellation);
    }

}
