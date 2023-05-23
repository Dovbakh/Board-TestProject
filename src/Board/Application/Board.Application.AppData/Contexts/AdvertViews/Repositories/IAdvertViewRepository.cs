using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertViews.Repositories
{
    /// <summary>
    /// Репозиторий для работы с просмотрами обьявлений.
    /// </summary>
    public interface IAdvertViewRepository
    {
        /// <summary>
        /// Получить количество просмотров обьявления.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Количество просмотров.</returns>
        Task<int> GetCountAsync(Guid advertId, CancellationToken cancellation);

        /// <summary>
        /// Добавить просмотр обьявления, если такого еще не существует.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="visitorId">Идентификатор посетителя (авторизованного или неавторизованного).</param>
        /// <param name="isRegistered">Индикатор является ли пользователь авторизованным.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор просмотра обьявления - нового или существующего.</returns>
        Task<Guid> AddIfNotExistsAsync(Guid advertId, Guid visitorId, bool isRegistered, CancellationToken cancellation);
     
    }
}
