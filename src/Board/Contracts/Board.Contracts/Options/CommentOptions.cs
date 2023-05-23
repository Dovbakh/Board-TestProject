using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    /// <summary>
    /// Опции сервиса для работы с отзывами.
    /// </summary>
    public class CommentOptions
    {
        /// <summary>
        /// Количество получаемых отзывов по умолчанию.
        /// </summary>
        public int ListDefaultCount { get; set; } = 10;
    }
}
