using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    /// <summary>
    /// Опции сервиса для работы с обьявлениями.
    /// </summary>
    public class AdvertOptions
    {
        /// <summary>
        /// Количество получаемых обьявлений по умолчанию.
        /// </summary>
        public int ListDefaultCount { get; set; } = 10;

        /// <summary>
        /// Количество получаемых отзывов обьявления по умолчанию.
        /// </summary>
        public int CommentListDefaultCount { get; set; } = 10;
    }
}
