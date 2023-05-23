using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    /// <summary>
    /// Опции для блокировки ресурса при добавлении обьявления в избранное.
    /// </summary>
    public class AdvertFavoriteAddLockOptions
    {
        /// <summary>
        /// Префикс ресурса.
        /// </summary>
        public string AdvertFavoriteAddKey { get; set; } = "AdvertFavoriteAddKey_";

        /// <summary>
        /// Время истечения блокировки ресурса.
        /// </summary>
        public TimeSpan Expire { get; set; }

        /// <summary>
        /// Время ожидания разблокирования ресурса.
        /// </summary>
        public TimeSpan Wait { get; set; }

        /// <summary>
        /// Время между попытками получения доступа к ресурсу.
        /// </summary>
        public TimeSpan Retry { get; set; }
    }
}
