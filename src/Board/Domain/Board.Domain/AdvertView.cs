using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    /// <summary>
    /// Просмотр обьявления.
    /// </summary>
    public class AdvertView
    {
        /// <summary>
        /// Идентификатор просмотра.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор обьявления.
        /// </summary>
        public Guid AdvertId { get; set; }

        /// <summary>
        /// Идентификатор посетителя.
        /// </summary>
        public Guid VisitorId { get; set; }

        /// <summary>
        /// Дата просмотра.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Индикатор авторизации посетителя.
        /// </summary>
        public bool IsRegistered { get; set; }

        /// <summary>
        /// Обьявление.
        /// </summary>
        public Advert Advert { get; set; }
    }
}
