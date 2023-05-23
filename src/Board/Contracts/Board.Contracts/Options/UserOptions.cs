using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    /// <summary>
    /// Опции сервиса для работы с пользователями.
    /// </summary>
    public class UserOptions
    {
        /// <summary>
        /// Количество получаемых пользователей по умолчанию.
        /// </summary>
        public int ListDefaultCount { get; set; } = 10;

        /// <summary>
        /// Количество получаемых обьявлений пользователя по умолчанию.
        /// </summary>
        public int AdvertListDefaultCount { get; set; } = 10;

        /// <summary>
        /// Количество получаемых отзывов пользователя по умолчанию.
        /// </summary>
        public int CommentListDefaultCount { get; set; } = 10;
    }
}
