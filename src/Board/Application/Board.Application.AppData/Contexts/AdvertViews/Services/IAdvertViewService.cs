using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertViews.Services
{
    /// <summary>
    /// Сервис для работы с просмотрами обьявлений.
    /// </summary>
    public interface IAdvertViewService
    {
        /// <summary>
        /// Получить количество просмотров обьявления.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Количество просмотров.</returns>
        Task<int> GetCountAsync(Guid advertId, CancellationToken cancellation);

        /// <summary>
        /// Добавить просмотр обьявления.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор просмотра обьявления.</returns>
        Task<Guid> AddIfNotExistsAsync(Guid advertId, CancellationToken cancellation);       
    }
}
